
namespace Anycmd.Host.AC.MessageHandlers
{
    using Commands;
    using Exceptions;
    using Identity;
    using Identity.Messages;
    using Repositories;
    using System.Linq;

    public class UpdateAccountCommandHandler : CommandHandler<UpdateAccountCommand>
    {
        private readonly AppHost host;

        public UpdateAccountCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(UpdateAccountCommand command)
        {
            var accountRepository = host.GetRequiredService<IRepository<Account>>();
            if (accountRepository.FindAll()
                .Where(a => a.Code == command.Input.Code && a.Id != command.Input.Id)
                .Any())
            {
                throw new ValidationException("用户编码重复");
            }
            var entity = accountRepository.GetByKey(command.Input.Id);
            if (entity == null)
            {
                throw new NotExistException();
            }
            if (command.Input.OrganizationCode != entity.OrganizationCode)
            {
                if (string.IsNullOrEmpty(command.Input.OrganizationCode))
                {
                    throw new CoreException("用户必须属于一个组织结构");
                }
                OrganizationState organization;
                if (!host.OrganizationSet.TryGetOrganization(command.Input.OrganizationCode, out organization))
                {
                    throw new CoreException("意外的组织结构码" + command.Input.OrganizationCode);
                }
            }
            entity.Update(command.Input);
            accountRepository.Update(entity);
            accountRepository.Context.Commit();
            AccountState devAccount;
            if (host.SysUsers.TryGetDevAccount(entity.Id, out devAccount))
            {
                host.EventBus.Publish(new DeveloperUpdatedEvent(entity));
            }
            host.EventBus.Publish(new AccountUpdatedEvent(entity));
            host.EventBus.Commit();
        }
    }
}

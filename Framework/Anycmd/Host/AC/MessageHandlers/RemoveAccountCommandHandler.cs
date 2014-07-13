
namespace Anycmd.Host.AC.MessageHandlers
{
    using Commands;
    using Exceptions;
    using Identity;
    using Identity.Messages;
    using Repositories;

    public class RemoveAccountCommandHandler : CommandHandler<RemoveAccountCommand>
    {
        private readonly AppHost host;

        public RemoveAccountCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(RemoveAccountCommand command)
        {
            var accountRepository = host.GetRequiredService<IRepository<Account>>();
            AccountState developer;
            if (host.SysUsers.TryGetDevAccount(command.EntityID, out developer))
            {
                throw new ValidationException("该账户是开发人员，删除该账户之前需先删除该开发人员");
            }
            var entity = accountRepository.GetByKey(command.EntityID);
            if (entity == null)
            {
                return;
            }
            accountRepository.Remove(entity);
            accountRepository.Context.Commit();
            host.EventBus.Publish(new AccountRemovedEvent(entity));
            host.EventBus.Commit();
        }
    }
}

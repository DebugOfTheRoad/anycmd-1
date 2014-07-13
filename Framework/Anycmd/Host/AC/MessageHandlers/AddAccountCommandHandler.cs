﻿
namespace Anycmd.Host.AC.MessageHandlers
{
    using Commands;
    using Exceptions;
    using Identity;
    using Identity.Messages;
    using Repositories;
    using System;
    using System.Linq;

    public class AddAccountCommandHandler : CommandHandler<AddAccountCommand>
    {
        private readonly AppHost host;

        public AddAccountCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(AddAccountCommand command)
        {
            var accountRepository = host.GetRequiredService<IRepository<Account>>();
            if (string.IsNullOrEmpty(command.Input.OrganizationCode))
            {
                throw new CoreException("用户必须属于一个组织结构");
            }
            OrganizationState organization;
            if (!host.OrganizationSet.TryGetOrganization(command.Input.OrganizationCode, out organization))
            {
                throw new CoreException("意外的组织结构码" + command.Input.OrganizationCode);
            }
            if (accountRepository.FindAll().Any(a => a.Code == command.Input.Code && a.Id != command.Input.Id))
            {
                throw new ValidationException("用户编码重复");
            }
            if (accountRepository.FindAll()
                .Where(a => a.LoginName == command.Input.LoginName)
                .Any())
            {
                throw new ValidationException("重复的登录名");
            }
            var entity = Account.Create(command.Input);
            if (string.IsNullOrEmpty(command.Input.Password))
            {
                throw new ValidationException("新密码不能为空");
            }
            var passwordEncryptionService = host.GetRequiredService<IPasswordEncryptionService>();
            entity.Password = passwordEncryptionService.Encrypt(command.Input.Password);
            entity.LastPasswordChangeOn = DateTime.Now;

            accountRepository.Add(entity);
            accountRepository.Context.Commit();
            host.EventBus.Publish(new AccountAddedEvent(entity));
            host.EventBus.Commit();
        }
    }
}


namespace Anycmd.Host.AC.MessageHandlers
{
    using Commands;
    using Exceptions;
    using Host;
    using Identity;
    using Identity.Messages;
    using Repositories;
    using System;
    using System.Linq;

    public class ChangePasswordCommandHandler : CommandHandler<ChangePasswordCommand>
    {
        private readonly AppHost host;

        public ChangePasswordCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(ChangePasswordCommand command)
        {
            var accountRepository = host.GetRequiredService<IRepository<Account>>();
            if (command.Input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (string.IsNullOrEmpty(command.Input.LoginName))
            {
                throw new ValidationException("登录名不能为空");
            }

            var entity = accountRepository.FindAll().FirstOrDefault(a => a.LoginName == command.Input.LoginName);
            if (entity == null)
            {
                throw new NotExistException("用户名" + command.Input.LoginName + "不存在");
            }
            bool loginNameChanged = !string.Equals(command.Input.LoginName, entity.LoginName);
            AccountState developer;
            if (host.SysUsers.TryGetDevAccount(command.Input.LoginName, out developer) && !host.User.IsDeveloper())
            {
                throw new ValidationException("对不起，您不能修改开发人员的密码。");
            }
            if (!host.User.IsDeveloper() && "admin".Equals(entity.LoginName, StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException("对不起，您无权修改admin账户的密码");
            }
            #region 更改登录名
            if (string.IsNullOrEmpty(command.Input.LoginName))
            {
                throw new ValidationException("登录名不能为空");
            }
            if (loginNameChanged)
            {
                entity.LoginName = command.Input.LoginName;
            }
            #endregion
            #region 更改密码
            if (string.IsNullOrEmpty(command.Input.OldPassword))
            {
                throw new ValidationException("旧密码不能为空");
            }
            if (string.IsNullOrEmpty(command.Input.NewPassword))
            {
                throw new ValidationException("新密码不能为空");
            }
            var passwordEncryptionService = host.GetRequiredService<IPasswordEncryptionService>();
            var oldPwd = passwordEncryptionService.Encrypt(command.Input.OldPassword);
            if (string.Equals(entity.Password, oldPwd))
            {
                throw new ValidationException("旧密码不正确");
            }
            var newPassword = passwordEncryptionService.Encrypt(command.Input.NewPassword);
            if (oldPwd != newPassword)
            {
                entity.Password = newPassword;
                entity.LastPasswordChangeOn = DateTime.Now;
                host.EventBus.Publish(new PasswordUpdatedEvent(entity));
            }
            #endregion
            if (loginNameChanged)
            {
                host.EventBus.Publish(new LoginNameChangedEvent(entity));
                if (host.SysUsers.TryGetDevAccount(entity.Id, out developer))
                {
                    host.MessageDispatcher.DispatchMessage(new DeveloperUpdatedEvent(entity));
                }
            }
            accountRepository.Update(entity);
            accountRepository.Context.Commit();
            host.EventBus.Commit();
        }
    }
}

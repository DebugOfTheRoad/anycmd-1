
namespace Anycmd.Host.AC.MessageHandlers
{
    using Commands;
    using Host;
    using Identity;
    using Identity.Messages;
    using Logging;
    using Repositories;
    using System.Web;
    using System.Web.Security;
    using Transactions;
    using Util;

    public class AccountSignOutCommandHandler : CommandHandler<AccountSignOutCommand>
    {
        private readonly AppHost host;

        public AccountSignOutCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(AccountSignOutCommand command)
        {
            string msg;
            var accountRepository = host.GetRequiredService<IRepository<Account>>();
            var visitingLogRepository = host.GetRequiredService<IRepository<VisitingLog>>();
            var userSessionStorage = host.GetRequiredService<IUserSessionStorage>();
            try
            {
                using (var coordinator = TransactionCoordinatorFactory.Create(accountRepository.Context, host.EventBus))
                {
                    msg = "退出成功";
                    if (!host.UserSession.Principal.Identity.IsAuthenticated)
                    {
                        return;
                    }
                    var entity = accountRepository.GetByKey(host.UserSession.GetAccountID());
                    var visitingLog = host.UserSession.GetData<VisitingLog>("UserContext_Current_VisitingLog");
                    if (visitingLog != null)
                    {
                        visitingLog.StateCode = (int)VisitState.LogOut;
                        visitingLog.ReasonPhrase = VisitState.LogOut.ToName();
                        visitingLog.Description = msg;
                        visitingLog.VisitedOn = SystemTime.Now();
                        visitingLogRepository.Update(visitingLog);
                    }
                    if (HttpContext.Current != null)
                    {
                        FormsAuthentication.SignOut();
                    }
                    userSessionStorage.Clear();
                    host.EventBus.Publish(new AccountLogoutedEvent(entity));
                    coordinator.Commit();
                    return;
                }
            }
            catch
            {
                msg = "退出失败";
                throw;
            }
        }
    }
}


namespace Anycmd.Host.AC.MessageHandlers
{
    using AC.Identity;
    using Commands;
    using Exceptions;
    using Host;
    using Identity.Messages;
    using Logging;
    using Repositories;
    using System;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Security;
    using Transactions;
    using Util;

    public class AccountSignInCommandHandler : CommandHandler<AccountSignInCommand>
    {
        private readonly AppHost host;

        public AccountSignInCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(AccountSignInCommand command)
        {
            if (string.IsNullOrEmpty(command.LoginName) || string.IsNullOrEmpty(command.Password))
            {
                throw new ValidationException("用户名和密码不能为空");
            }
            else
            {
                var accountRepository = host.GetRequiredService<IRepository<Account>>();
                var visitingLogRepository = host.GetRequiredService<IRepository<VisitingLog>>();
                var passwordEncryptionService = host.GetRequiredService<IPasswordEncryptionService>();
                string msg;
                try
                {
                    using (var coordinator = TransactionCoordinatorFactory.Create(accountRepository.Context, visitingLogRepository.Context, host.EventBus))
                    {
                        msg = "登录成功";
                        if (string.IsNullOrEmpty(command.LoginName) || string.IsNullOrEmpty(command.Password))
                        {
                            msg = "用户名和密码不能为空";
                            return;
                        }
                        if (host.UserSession.Principal.Identity.IsAuthenticated)
                        {
                            return;
                        }
                        var password = passwordEncryptionService.Encrypt(command.Password);
                        VisitingLog visitingLog = new VisitingLog
                        {
                            Id = Guid.NewGuid(),
                            IPAddress = GetClientIP(),
                            LoginName = command.LoginName,
                            VisitedOn = null,
                            VisitOn = DateTime.Now
                        };
                        var account = accountRepository.FindAll().FirstOrDefault(a => a.LoginName == command.LoginName);
                        if (account == null)
                        {
                            msg = "用户名错误";
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        visitingLog.AccountID = account.Id;
                        msg = "密码错误";
                        if (password != account.Password)
                        {
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        if (account.IsEnabled == 0)
                        {
                            msg = "对不起，该账户已被禁用";
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        if (account.IsEnabled == 0)
                        {
                            msg = "对不起，" + account.Name + "已被禁用";
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        string auditState = account.AuditState == null ? account.AuditState : account.AuditState.ToLower();
                        DicState dic;
                        if (!host.DicSet.TryGetDic("auditStatus", out dic))
                        {
                            throw new CoreException("意外的字典编码auditStatus");
                        }
                        var auditStatusDic = host.DicSet.GetDicItems(dic);
                        if (!auditStatusDic.ContainsKey(auditState))
                        {
                            auditState = null;
                        }
                        if (auditState == null
                            || auditState == "notaudit")
                        {
                            msg = "对不起，该账户尚未审核";
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        if (auditState != null && auditState == "auditnotpass")
                        {
                            msg = "对不起，该账户未通过审核";
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        if (account.AllowStartTime.HasValue && SystemTime.Now() < account.AllowStartTime.Value)
                        {
                            msg = "对不起，该账户的允许登录开始时间还没到。请在" + account.AllowStartTime.ToString() + "后登录";
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        if (account.AllowEndTime.HasValue && SystemTime.Now() > account.AllowEndTime.Value)
                        {
                            msg = "对不起，该账户的允许登录时间已经过期";
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        if (account.LockEndTime.HasValue || account.LockStartTime.HasValue)
                        {
                            DateTime lockStartTime = account.LockStartTime ?? DateTime.MinValue;
                            DateTime lockEndTime = account.LockEndTime ?? DateTime.MaxValue;
                            if (SystemTime.Now() > lockStartTime && SystemTime.Now() < lockEndTime)
                            {
                                msg = "对不起，该账户暂被锁定";
                                visitingLog.Description = msg;
                                visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                                visitingLog.StateCode = (int)VisitState.LogOnFail;
                                visitingLogRepository.Add(visitingLog);
                                return;
                            }
                        }
                        visitingLog.Description = "登录成功";
                        visitingLog.ReasonPhrase = VisitState.Logged.ToName();
                        visitingLog.StateCode = (int)VisitState.Logged;

                        if (account.PreviousLoginOn.HasValue && account.PreviousLoginOn.Value >= SystemTime.Now())
                        {
                            msg = "检测到您的上次登录时间在未来。这可能是因为本站点服务器的时间落后导致的，请联系管理员。";
                            visitingLog.Description = msg;
                            visitingLog.ReasonPhrase = VisitState.LogOnFail.ToName();
                            visitingLog.StateCode = (int)VisitState.LogOnFail;
                            visitingLogRepository.Add(visitingLog);
                            return;
                        }
                        account.PreviousLoginOn = SystemTime.Now();
                        if (!account.FirstLoginOn.HasValue)
                        {
                            account.FirstLoginOn = SystemTime.Now();
                        }
                        account.LoginCount = (account.LoginCount ?? 0) + 1;
                        account.IPAddress = visitingLog.IPAddress;

                        accountRepository.Update(account);
                        bool createPersistentCookie = "rememberMe".Equals(command.RememberMe, StringComparison.OrdinalIgnoreCase);
                        FormsAuthentication.SetAuthCookie(command.LoginName, createPersistentCookie);
                        host.UserSession.SetData("UserContext_Current_VisitingLog", visitingLog);
                        host.EventBus.Publish(new AccountLoginedEvent(account));
                        visitingLogRepository.Add(visitingLog);
                        coordinator.Commit();
                        return;
                    }
                }
                catch
                {
                    msg = "登录失败";
                    throw;
                }
            }
        }

        private string GetClientIP()
        {
            if (HttpContext.Current == null)
            {
                return IPAddress.Loopback.ToString();
            }
            string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (null == ip || ip == String.Empty)
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (null == ip || ip == String.Empty)
            {
                ip = HttpContext.Current.Request.UserHostAddress;
            }
            if (ip == "::1")
            {
                ip = "127.0.0.1";
            }

            return ip;
        }
    }
}

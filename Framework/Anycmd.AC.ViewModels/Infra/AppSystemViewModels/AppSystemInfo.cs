
namespace Anycmd.AC.Infra.ViewModels.AppSystemViewModels
{
    using Exceptions;
    using Host;
    using Model;
    using System;
    using System.Collections.Generic;

    public class AppSystemInfo : Dictionary<string, object>
    {
        private AppSystemInfo() { }

        public static AppSystemInfo Create(DicReader dic)
        {
            if (dic == null)
            {
                return null;
            }
            var data = new AppSystemInfo();
            foreach (var item in dic)
            {
                data.Add(item.Key, item.Value);
            }
            AccountState principal;
            if (!dic.Host.SysUsers.TryGetDevAccount((Guid)data["PrincipalID"], out principal))
            {
                throw new CoreException("意外的开发人员标识" + data["PrincipalID"]);
            }
            if (!data.ContainsKey("PrincipalName"))
            {
                data.Add("PrincipalName", principal.LoginName);
            }
            if (!data.ContainsKey("IsEnabledName"))
            {
                data.Add("IsEnabledName", dic.Host.Translate("AC", "AppSystem", "IsEnabledName", data["IsEnabled"].ToString()));
            }

            return data;
        }
    }
}

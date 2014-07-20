
namespace Anycmd.AC.ViewModels.GroupViewModels
{
    using System;
    using System.Collections.Generic;

    public class GroupInfo : Dictionary<string, object>
    {
        public GroupInfo(IAppHost host, Dictionary<string, object> dic)
        {
            if (dic == null)
            {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic)
            {
                this.Add(item.Key, item.Value);
            }
            if (!this.ContainsKey("IsEnabledName"))
            {
                this.Add("IsEnabledName", host.Translate("AC", "Group", "IsEnabledName", this["IsEnabled"].ToString()));
            }
        }
    }
}


namespace Anycmd.EDI.ViewModels.InfoDicViewModels {
    using System;
    using System.Collections.Generic;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public partial class InfoDicItemInfo : Dictionary<string, object> {
        public InfoDicItemInfo(AppHost host, Dictionary<string, object> dic) {
            if (dic == null) {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic) {
                this.Add(item.Key, item.Value);
            }
            if (!this.ContainsKey("DeletionStateName")) {
                this.Add("DeletionStateName", host.Translate("EDI", "InfoDic", "DeletionStateName", (int)this["DeletionStateCode"]));
            }
            if (!this.ContainsKey("IsEnabledName")) {
                this.Add("IsEnabledName", host.Translate("EDI", "InfoDic", "IsEnabledName", (int)this["IsEnabled"]));
            }
        }
    }
}

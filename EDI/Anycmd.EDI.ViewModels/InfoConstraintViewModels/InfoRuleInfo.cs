
namespace Anycmd.EDI.ViewModels.InfoConstraintViewModels {
    using Anycmd.Host.EDI;
    using Exceptions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class InfoRuleInfo : Dictionary<string, object> {
        public InfoRuleInfo() { }

        public InfoRuleInfo(Dictionary<string, object> dic) {
            if (dic == null) {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic) {
                this.Add(item.Key, item.Value);
            }
            InfoRuleState infoRule;
            if (!NodeHost.Instance.InfoRules.TryGetInfoRule((Guid)this["Id"], out infoRule)) {
                throw new CoreException("�������Ϣ�����ʶ" + this["Id"]);
            }
            if (!this.ContainsKey("Name")) {
                this.Add("Name", infoRule.GetType().Name);
            }
            if (!this.ContainsKey("FullName")) {
                this.Add("FullName", infoRule.GetType().FullName);
            }
            if (!this.ContainsKey("Title")) {
                this.Add("Title", infoRule.InfoRule.Title);
            }
            if (!this.ContainsKey("Description")) {
                this.Add("Description", infoRule.InfoRule.Description);
            }
            if (!this.ContainsKey("Author")) {
                this.Add("Author", infoRule.InfoRule.Author);
            }
        }
    }
}

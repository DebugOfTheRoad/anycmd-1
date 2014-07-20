
namespace Anycmd.EDI.ViewModels.OntologyViewModels {
    using System;
    using System.Collections.Generic;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public partial class OntologyInfo : Dictionary<string, object> {
        public OntologyInfo() { }

        public OntologyInfo(IAppHost host, Dictionary<string, object> dic)
        {
            if (dic == null) {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic) {
                this.Add(item.Key, item.Value);
            }
            if (!this.ContainsKey("DeletionStateName")) {
                this.Add("DeletionStateName", host.Translate("EDI", "Ontology", "DeletionStateName", (int)this["DeletionStateCode"]));
            }
            if (!this.ContainsKey("IsEnabledName")) {
                this.Add("IsEnabledName", host.Translate("EDI", "Ontology", "IsEnabledName", (int)this["IsEnabled"]));
            }
            if (!this.ContainsKey("IsOrganizationalEntityName")) {
                this.Add("IsOrganizationalEntityName", host.Translate("EDI", "Ontology", "IsOrganizationalEntityName", (bool)this["IsOrganizationalEntity"]));
            }
            if (!this.ContainsKey("IsLogicalDeletionEntityName")) {
                this.Add("IsLogicalDeletionEntityName", host.Translate("EDI", "Ontology", "IsLogicalDeletionEntityName", (bool)this["IsLogicalDeletionEntity"]));
            }
        }
    }
}

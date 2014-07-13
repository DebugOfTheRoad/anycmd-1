
namespace Anycmd.EDI.ViewModels.BatchViewModels {
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using Anycmd.Host.EDI;

    /// <summary>
    /// 
    /// </summary>
    public partial class BatchTr : Dictionary<string, object> {
        public BatchTr() { }

        public BatchTr(Dictionary<string, object> dic) {
            if (dic == null) {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic) {
                this.Add(item.Key, item.Value);
            }
            OntologyDescriptor ontology;
            if (!NodeHost.Instance.Ontologies.TryGetOntology((Guid)this["OntologyID"], out ontology)) {
                throw new CoreException("意外的本体标识" + this["OntologyID"]);
            }
            if (!this.ContainsKey("OntologyCode")) {
                this.Add("OntologyCode", ontology.Ontology.Code);
            }
            if (!this.ContainsKey("OntologyName")) {
                this.Add("OntologyName", ontology.Ontology.Name);
            }
            NodeDescriptor node;
            if (!NodeHost.Instance.Nodes.TryGetNodeByID(this["NodeID"].ToString(), out node)) {
                throw new CoreException("意外的节点标识" + this["NodeID"]);
            }
            if (!this.ContainsKey("OntologyCode")) {
                this.Add("OntologyCode", ontology.Ontology.Code);
            }
            if (!this.ContainsKey("OntologyName")) {
                this.Add("OntologyName", ontology.Ontology.Name);
            }
            if (!this.ContainsKey("NodeCode")) {
                this.Add("NodeCode", node.Node.Code);
            }
            if (!this.ContainsKey("NodeName")) {
                this.Add("NodeName", node.Node.Name);
            }
        }
    }
}

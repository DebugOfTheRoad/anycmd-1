﻿
namespace Anycmd.EDI.ViewModels.BatchViewModels
{
    using Exceptions;
    using Host.EDI;
    using Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class BatchInfo : Dictionary<string, object>
    {
        public BatchInfo() { }

        public BatchInfo(DicReader dic)
        {
            if (dic == null)
            {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic)
            {
                this.Add(item.Key, item.Value);
            }
            OntologyDescriptor ontology;
            if (!dic.Host.Ontologies.TryGetOntology((Guid)this["OntologyID"], out ontology))
            {
                throw new CoreException("意外的本体标识" + this["OntologyID"]);
            }
            if (!this.ContainsKey("OntologyCode"))
            {
                this.Add("OntologyCode", ontology.Ontology.Code);
            }
            if (!this.ContainsKey("OntologyName"))
            {
                this.Add("OntologyName", ontology.Ontology.Name);
            }
            NodeDescriptor node;
            if (!dic.Host.Nodes.TryGetNodeByID(this["NodeID"].ToString(), out node))
            {
                throw new CoreException("意外的节点标识" + this["NodeID"]);
            }
            if (!this.ContainsKey("NodeCode"))
            {
                this.Add("NodeCode", node.Node.Code);
            }
            if (!this.ContainsKey("NodeName"))
            {
                this.Add("NodeName", node.Node.Name);
            }
        }
    }
}

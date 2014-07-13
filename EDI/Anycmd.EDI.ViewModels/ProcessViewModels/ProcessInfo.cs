
namespace Anycmd.EDI.ViewModels.ProcessViewModels
{
    using System;
    using Exceptions;
    using System.Collections.Generic;
    using Anycmd.Host.EDI;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessInfo : Dictionary<string, object>
    {
        public ProcessInfo() { }

        public ProcessInfo(Dictionary<string, object> dic)
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
            if (!NodeHost.Instance.Ontologies.TryGetOntology((Guid)this["OntologyID"], out ontology))
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
            ProcessDescriptor _process;
            if (!NodeHost.Instance.Processs.TryGetProcess((Guid)this["Id"], out _process))
            {
                throw new CoreException("意外的进程标识" + this["Id"]);
            }
            if (!this.ContainsKey("WebApiBaseAddress"))
            {
                this.Add("WebApiBaseAddress", _process.WebApiBaseAddress);
            }
        }
    }
}

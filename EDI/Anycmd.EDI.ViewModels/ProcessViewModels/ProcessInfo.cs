
namespace Anycmd.EDI.ViewModels.ProcessViewModels
{
    using Exceptions;
    using Host.EDI;
    using Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessInfo : Dictionary<string, object>
    {
        private ProcessInfo() { }

        public ProcessInfo(DicReader dic)
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
            ProcessDescriptor _process;
            if (!dic.Host.Processs.TryGetProcess((Guid)this["Id"], out _process))
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

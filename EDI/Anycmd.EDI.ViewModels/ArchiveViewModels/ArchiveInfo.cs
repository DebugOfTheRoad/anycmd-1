
namespace Anycmd.EDI.ViewModels.ArchiveViewModels
{
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using Anycmd.Host.EDI;

    /// <summary>
    /// 
    /// </summary>
    public partial class ArchiveInfo : Dictionary<string, object>
    {
        public ArchiveInfo() { }

        public ArchiveInfo(Dictionary<string, object> dic)
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
            if (!this.ContainsKey("CatalogName"))
            {
                this.Add("CatalogName", string.Format(
                                "Archive{0}{1}_{2}",
                                ontology.Ontology.Code,
                                ((DateTime)this["ArchiveOn"]).ToString("yyyyMMdd"),
                                this["NumberID"]));
            }
        }
    }
}


namespace Anycmd.EDI.ViewModels.ArchiveViewModels
{
    using Exceptions;
    using Host.EDI;
    using Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class ArchiveInfo : Dictionary<string, object>
    {
        public ArchiveInfo() { }

        public ArchiveInfo(DicReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            foreach (var item in reader)
            {
                this.Add(item.Key, item.Value);
            }
            OntologyDescriptor ontology;
            if (!reader.Host.Ontologies.TryGetOntology((Guid)this["OntologyID"], out ontology))
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


namespace Anycmd.EDI.ViewModels.ArchiveViewModels {
    using System;
    using Exceptions;
    using Anycmd.Host.EDI;

    /// <summary>
    /// 
    /// </summary>
    public partial class ArchiveTr {
        private OntologyDescriptor _ontology;

        public ArchiveTr() { }

        public static ArchiveTr Create(ArchiveState archive)
        {
            return new ArchiveTr {
                ArchiveOn = archive.ArchiveOn,
                CreateOn = archive.CreateOn,
                CreateBy = archive.CreateBy,
                CreateUserID = archive.CreateUserID,
                DataSource = archive.DataSource,
                Id = archive.Id,
                NumberID = archive.NumberID,
                OntologyID = archive.OntologyID,
                Title = archive.Title,
                UserID = archive.UserID
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NumberID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ArchiveOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? CreateUserID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string OntologyCode {
            get { return Ontology.Ontology.Code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OntologyName {
            get { return Ontology.Ontology.Name; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CatalogName {
            get {
                var _catalogName = string.Format(
                            "Archive{0}{1}_{2}",
                            Ontology.Ontology.Code,
                            this.ArchiveOn.ToString("yyyyMMdd"),
                            this.NumberID.ToString());
                return _catalogName;
            }
        }
        private OntologyDescriptor Ontology {
            get {
                if (_ontology == null) {
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(this.OntologyID, out _ontology)) {
                        throw new CoreException("意外的本体标识" + this.OntologyID);
                    }
                }
                return _ontology;
            }
        }
    }
}

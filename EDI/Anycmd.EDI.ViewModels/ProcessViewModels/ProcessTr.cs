
namespace Anycmd.EDI.ViewModels.ProcessViewModels {
    using Anycmd.Host.EDI;
    using Exceptions;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessTr {
        private ProcessDescriptor _process;

        public ProcessTr() { }

        public static ProcessTr Create(ProcessDescriptor process) {
            return new ProcessTr {
                CreateOn = process.Process.CreateOn,
                Id = process.Process.Id,
                IsEnabled = process.Process.IsEnabled,
                Name = process.Process.Name,
                NetPort = process.Process.NetPort,
                OntologyID = process.Process.OntologyID,
                OrganizationCode = process.Process.OrganizationCode,
                Type = process.Process.Type
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NetPort { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OntologyCode {
            get {
                return this.Ontology.Ontology.Code;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OntologyName {
            get {
                return this.Ontology.Ontology.Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrganizationCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateOn { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WebApiBaseAddress {
            get {
                if (_process == null) {
                    if (!NodeHost.Instance.Processs.TryGetProcess(this.Id, out _process)) {
                        throw new CoreException("意外的进程标识" + this.Id);
                    }
                }
                return _process.WebApiBaseAddress;
            }
        }

        private OntologyDescriptor _ontology;

        private OntologyDescriptor Ontology {
            get {
                if (_ontology == null) {
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(this.OntologyID, out _ontology)) {
                        throw new ValidationException("意外的本体标识" + this.OntologyID);
                    }
                }
                return _ontology;
            }
        }
    }
}

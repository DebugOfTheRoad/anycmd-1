
namespace Anycmd.EDI.ViewModels.OntologyViewModels {
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationAssignActionTr {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ActionID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OrganizationID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyOrganizationID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsAudit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IsAllowed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActionIsAllowed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActionIsAudit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Verb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
    }
}


namespace Anycmd.EDI.ViewModels.NodeViewModels {
    using System;

    /// <summary>
    /// 
    /// </summary>
    public partial class NodeElementAssignActionTr {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid NodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ElementID { get; set; }
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
        public string ElementCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ElementName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAllowed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAudit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActionIsAllow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ElementActionIsAudit { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ElementActionIsAllow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Verb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        public int SortCode { get; set; }
    }
}

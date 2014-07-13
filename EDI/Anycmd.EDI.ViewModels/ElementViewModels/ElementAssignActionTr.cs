
namespace Anycmd.EDI.ViewModels.ElementViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public partial class ElementAssignActionTr
    {
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
        public Guid ElementID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ElementCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ActionIsAllow { get; set; }
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
        public string Verb { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ElementName { get; set; }
    }
}

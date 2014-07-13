
namespace Anycmd.EDI.ViewModels.ElementViewModels {
    using System;

    /// <summary>
    /// 
    /// </summary>
    public partial class NodeAssignElementTr {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid ElementID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid NodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
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
        public bool IsAssigned { get; set; }

        public bool IsInfoIDItem { get; set; }

        public bool ElementIsInfoIDItem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateOn { get; set; }
    }
}

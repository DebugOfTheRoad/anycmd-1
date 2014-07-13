
namespace Anycmd.EDI.ViewModels.NodeViewModels {
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistNodeElementActions : GetPlistResult {
        /// <summary>
        /// 
        /// </summary>
        public Guid nodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid elementID { get; set; }
    }
}

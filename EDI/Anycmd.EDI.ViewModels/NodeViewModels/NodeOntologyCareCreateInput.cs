using System;
using System.ComponentModel.DataAnnotations;

namespace Anycmd.EDI.ViewModels.NodeViewModels
{
    using Host.EDI.ValueObjects;
    using Model;
    using AC;

    /// <summary>
    /// 
    /// </summary>
    public class NodeOntologyCareCreateInput : ManagedPropertyValues, IInputModel, INodeOntologyCareCreateInput
    {
        private Guid? _id = null;

        public Guid? Id
        {
            get
            {
                if (_id == null)
                {
                    _id = Guid.NewGuid();
                }
                return _id;
            }
            set { _id = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid NodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid OntologyID { get; set; }
    }
}

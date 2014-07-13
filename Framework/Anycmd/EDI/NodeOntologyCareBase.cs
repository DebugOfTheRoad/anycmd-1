
namespace Anycmd.EDI {
    using Exceptions;
    using Model;
    using System;

    public abstract class NodeOntologyCareBase : EntityBase, INodeOntologyCare {
        #region Private Fields
        private Guid _nodeID;
        private Guid _ontologyID;
        #endregion

        protected NodeOntologyCareBase() { }

        #region Public Properties
        /// <summary>
        /// 节点ID
        /// </summary>
        public Guid NodeID {
            get { return _nodeID; }
            set {
                if (value != _nodeID) {
                    if (_nodeID != Guid.Empty) {
                        throw new CoreException("关联节点不能更改");
                    }
                    _nodeID = value;
                }
            }
        }
        /// <summary>
        /// 本体主键
        /// </summary>
        public Guid OntologyID {
            get { return _ontologyID; }
            set {
                if (value != _ontologyID) {
                    if (_ontologyID != Guid.Empty) {
                        throw new CoreException("关联本体不能更改");
                    }
                    _ontologyID = value;
                }
            }
        }
        #endregion
    }
}

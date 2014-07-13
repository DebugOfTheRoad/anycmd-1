
namespace Anycmd.EDI {
    using Exceptions;
    using Model;
    using System;

    public abstract class NodeElementActionBase : EntityBase, INodeElementAction {
        private Guid _nodeID;
        private Guid _elementID;
        private Guid _actionID;
        private bool _isAudit;
        private bool _isAllowed;

        protected NodeElementActionBase() { }

        /// <summary>
        /// 
        /// </summary>
        public Guid NodeID {
            get { return _nodeID; }
            set {
                if (value != _nodeID) {
                    if (_nodeID != Guid.Empty) {
                        throw new CoreException("不能更改所属节点");
                    }
                    _nodeID = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid ElementID {
            get { return _elementID; }
            set {
                if (value != _elementID) {
                    if (_elementID != Guid.Empty) {
                        throw new CoreException("不能更改所属元素");
                    }
                    _elementID = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActionID {
            get { return _actionID; }
            set {
                if (value != _actionID) {
                    if (_actionID != Guid.Empty) {
                        throw new CoreException("不能更改所属动作");
                    }
                    _actionID = value;
                }
            }
        }
        /// <summary>
        /// 是否需要审核
        /// </summary>
        public bool IsAudit {
            get { return _isAudit; }
            set {
                if (value != _isAudit) {
                    _isAudit = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAllowed {
            get { return _isAllowed; }
            set {
                if (value != _isAllowed) {
                    _isAllowed = value;
                }
            }
        }
    }
}

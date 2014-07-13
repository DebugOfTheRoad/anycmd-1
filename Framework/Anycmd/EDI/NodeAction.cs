
namespace Anycmd.EDI {
    using Exceptions;
    using Host.AC;
    using Model;
    using System;
    using Util;

    public sealed class NodeAction : EntityBase, INodeAction {
        private Guid _nodeID;
        private Guid _actionID;
        private string _isAudit;
        private string _isAllowed;

        public NodeAction() { }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

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
        public string IsAudit {
            get { return _isAudit; }
            set {
                if (value != _isAudit) {
                    _isAudit = value;
                    AuditType auditType;
                    if (!value.TryParse(out auditType)) {
                        throw new CoreException("意外的AuditType:" + value);
                    }
                    this.AuditType = auditType;
                }
            }
        }

        /// <summary>
        /// 是否需要审核
        /// </summary>
        public AuditType AuditType { get; private set; }

        /// <summary>
        /// 是否允许
        /// </summary>
        public string IsAllowed {
            get { return _isAllowed; }
            set {
                if (value != _isAllowed) {
                    _isAllowed = value;
                    AllowType allowType;
                    if (!value.TryParse(out allowType)) {
                        throw new CoreException("意外的AllowType:" + value);
                    }
                    this.AllowType = allowType;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public AllowType AllowType { get; private set; }
    }
}
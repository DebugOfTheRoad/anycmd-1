﻿
namespace Anycmd.EDI {
    using Exceptions;
    using Host.AC;
    using Model;
    using System;
    using Util;

    public sealed class OrganizationAction : EntityBase, IOrganizationAction {
        private Guid _orgnizationID;
        private Guid _actionID;
        private string _isAudit;
        private string _isAllowed;

        public OrganizationAction() { }

        /// <summary>
        /// 
        /// </summary>
        public Guid OrganizationID {
            get { return _orgnizationID; }
            set {
                if (value != _orgnizationID) {
                    if (_orgnizationID != Guid.Empty) {
                        throw new CoreException("不能更改所属组织结构");
                    }
                    _orgnizationID = value;
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
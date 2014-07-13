
namespace Anycmd.EDI {
    using Exceptions;
    using Host.AC;
    using Model;
    using System;
    using Util;

    /// <summary>
    /// 本体动作领域实体基类。本体动作领域实体模型必须继承该类。
    /// <remarks>
    /// 领域实体模型充当了数据访问模型。
    /// </remarks>
    /// </summary>
    public abstract class ActionBase : EntityBase, IAction {
        #region Private Fields
        private string _code;
        private string _name;
        private Guid _ontologyID;
        private bool _isPersist;
        private string _isAllowed;
        private string _isAudit;
        private int _sortCode;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected ActionBase() {
        }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Verb {
            get { return _code; }
            set {
                if (_code != null) {
                    if (!string.Equals(_code, value, StringComparison.OrdinalIgnoreCase)) {
                        throw new CoreException("动作码不能更改");
                    }
                }
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new CoreException("动作码不能为空");
                }
                _code = value;
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name {
            get { return _name; }
            set {
                if (value != _name) {
                    _name = value;
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
                        throw new CoreException("不能更改所属本体");
                    }
                    _ontologyID = value;
                }
            }
        }
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
        public AuditType AuditType { get; private set; }
        /// <summary>
        /// 是否持久化到数据库
        /// </summary>
        public bool IsPersist {
            get { return _isPersist; }
            set {
                if (value != _isPersist) {
                    _isPersist = value;
                }
            }
        }

        public int SortCode {
            get { return _sortCode; }
            set {
                if (value != _sortCode) {
                    _sortCode = value;
                }
            }
        }
    }
}

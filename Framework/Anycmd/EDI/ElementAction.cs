
namespace Anycmd.EDI
{
    using Exceptions;
    using Host.AC;
    using Model;
    using System;
    using Util;

    /// <summary>
    /// 本体元素级动作。<see cref="IElementAction"/>
    /// </summary>
    public sealed class ElementAction : EntityBase, IElementAction
    {
        private Guid _elementID;
        private Guid _actionID;
        private string _isAudit;
        private string _isAllowed;

        public ElementAction() { }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ElementID
        {
            get { return _elementID; }
            set
            {
                if (value != _elementID)
                {
                    if (_elementID != Guid.Empty)
                    {
                        throw new CoreException("不能更改所属元素");
                    }
                    // 不要验证ElementID标识的本体元素的存在性，因为ElementAction与Element是一起存储和读取的。
                    _elementID = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid ActionID
        {
            get { return _actionID; }
            set
            {
                if (value != _actionID)
                {
                    if (_actionID != Guid.Empty)
                    {
                        throw new CoreException("不能更改所属动作");
                    }
                    _actionID = value;
                }
            }
        }

        /// <summary>
        /// 是否需要审核
        /// </summary>
        public string IsAudit
        {
            get { return _isAudit; }
            set
            {
                if (value != _isAudit)
                {
                    _isAudit = value;
                    AuditType auditType;
                    if (!value.TryParse(out auditType))
                    {
                        throw new CoreException("意外的AuditType:" + value);
                    }
                    this.AuditType = auditType;
                }
            }
        }

        /// <summary>
        /// 是否允许
        /// </summary>
        public string IsAllowed
        {
            get { return _isAllowed; }
            set
            {
                if (value != _isAllowed)
                {
                    _isAllowed = value;
                    AllowType allowType;
                    if (!value.TryParse(out allowType))
                    {
                        throw new CoreException("意外的AllowType:" + value);
                    }
                    this.AllowType = allowType;
                }
            }
        }

        /// <summary>
        /// 是否需要审核
        /// </summary>
        public AuditType AuditType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public AllowType AllowType { get; private set; }
    }
}

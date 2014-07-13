using System;

namespace Anycmd.Host.EDI
{
    using AC;
    using Anycmd.EDI;
    using Exceptions;
    using Util;

    public sealed class ElementActionState : IElementAction
    {
        private string _isAudit;
        private string _isAllowed;

        private ElementActionState() { }

        internal static ElementActionState Create(IElementAction elementAction)
        {
            if (elementAction == null)
            {
                throw new ArgumentNullException("elementAction");
            }
            var data = new ElementActionState
            {
                ActionID = elementAction.ActionID,
                ElementID = elementAction.ElementID,
                Id = elementAction.Id,
                IsAllowed = elementAction.IsAllowed,
                IsAudit = elementAction.IsAudit
            };

            return data;
        }

        public Guid Id { get; private set; }

        // 不要在这里验证ActionID的合法性
        public Guid ActionID { get; private set; }

        // 不要在这里验证ElementID的合法性
        public Guid ElementID { get; private set; }

        /// <summary>
        /// 是否需要审核
        /// </summary>
        public string IsAudit
        {
            get { return _isAudit; }
            private set
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
        /// 是否需要审核
        /// </summary>
        public AuditType AuditType { get; private set; }

        /// <summary>
        /// 是否允许
        /// </summary>
        public string IsAllowed
        {
            get { return _isAllowed; }
            private set
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
        /// 
        /// </summary>
        public AllowType AllowType { get; private set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!(obj is ElementActionState))
            {
                return false;
            }
            var left = this;
            var right = (ElementActionState)obj;

            return
                left.Id == right.Id &&
                left.ElementID == right.ElementID &&
                left.ActionID == right.ActionID &&
                left.AuditType == right.AuditType &&
                left.AllowType == right.AllowType;
        }

        public static bool operator ==(ElementActionState a, ElementActionState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(ElementActionState a, ElementActionState b)
        {
            return !(a == b);
        }
    }
}

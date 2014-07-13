
namespace Anycmd.Host.EDI
{
    using AC;
    using Anycmd.EDI;
    using Exceptions;
    using Hecp;
    using System;
    using Util;

    public sealed class ActionState : IAction
    {
        private Verb _actionVerb;
        private string _verb = null;
        private string _isAllowed;
        private string _isAudit;

        private ActionState() { }

        public static ActionState Create(IAction action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            return new ActionState
            {
                CreateOn = action.CreateOn,
                Description = action.Description,
                Id = action.Id,
                IsAudit = action.IsAudit,
                IsAllowed = action.IsAllowed,
                IsPersist = action.IsPersist,
                Name = action.Name,
                OntologyID = action.OntologyID,
                SortCode = action.SortCode,
                Verb = action.Verb
            };
        }

        public Guid Id { get; private set; }

        public Guid OntologyID { get; private set; }

        public string Verb
        {
            get { return _verb; }
            private set
            {
                if (!string.Equals(value, _verb, StringComparison.OrdinalIgnoreCase))
                {
                    _verb = value;
                    _actionVerb = new Verb(value);
                }
            }
        }

        public Verb ActionVerb
        {
            get { return _actionVerb; }
        }

        public string Name { get; private set; }
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
        public AuditType AuditType { get; private set; }
        public bool IsPersist { get; private set; }

        public int SortCode { get; private set; }

        public string Description { get; private set; }

        public DateTime? CreateOn { get; private set; }
    }
}

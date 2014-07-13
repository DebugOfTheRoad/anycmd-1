
namespace Anycmd.AC.Messages.Privileges
{
    using Anycmd.Host.AC.ValueObjects;
    using Model;
    using System;

    public class AccountPrivilegeCreateInput : EntityCreateInput, IInputModel, IAccountPrivilegeCreateInput
    {
        public virtual Guid AccountID { get; set; }

        public string PrivilegeType { get; set; }

        public virtual Guid PrivilegeInstanceID { get; set; }

        public string PrivilegeConstraint { get; set; }

        public int PrivilegeOrientation { get; set; }
    }
}

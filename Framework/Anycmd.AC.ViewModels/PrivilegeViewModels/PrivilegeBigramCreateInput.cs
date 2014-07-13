
namespace Anycmd.AC.ViewModels.PrivilegeViewModels
{
    using Host.AC.ValueObjects;
    using Model;
    using System;

    public class PrivilegeBigramCreateInput : EntityCreateInput, IInputModel, IPrivilegeBigramCreateInput
    {
        public string SubjectType { get; set; }

        public Guid SubjectInstanceID { get; set; }

        public string ObjectType { get; set; }

        public Guid ObjectInstanceID { get; set; }

        public string PrivilegeConstraint { get; set; }

        public int PrivilegeOrientation { get; set; }
    }
}

using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IPrivilegeBigramCreateInput : IEntityCreateInput
    {
        string SubjectType { get; }
        Guid SubjectInstanceID { get; }
        string ObjectType { get; }
        Guid ObjectInstanceID { get; }
        string PrivilegeConstraint { get; }
        int PrivilegeOrientation { get; }
    }
}

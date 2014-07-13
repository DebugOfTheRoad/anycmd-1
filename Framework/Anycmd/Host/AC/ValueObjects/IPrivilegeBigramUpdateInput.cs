using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IPrivilegeBigramUpdateInput : IEntityUpdateInput
    {
        string PrivilegeConstraint { get; }
    }
}

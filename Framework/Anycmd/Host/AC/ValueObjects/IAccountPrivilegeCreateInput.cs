
namespace Anycmd.Host.AC.ValueObjects
{
    using Model;
    using System;

    public interface IAccountPrivilegeCreateInput : IEntityCreateInput
    {
        Guid AccountID { get; }
        string PrivilegeType { get; }
        Guid PrivilegeInstanceID { get; }
        string PrivilegeConstraint { get; }
        int PrivilegeOrientation { get; }
    }
}

using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IAccountPrivilegeUpdateInput : IEntityUpdateInput
    {
        string PrivilegeConstraint { get; }
    }
}

using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface INodeElementCareCreateInput : IEntityCreateInput
    {
        Guid ElementID { get; }
        Guid NodeID { get; }
        bool IsInfoIDItem { get; }
    }
}

using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IProcessUpdateInput : IEntityUpdateInput
    {
        string Name { get; }

        int IsEnabled { get; }
    }
}

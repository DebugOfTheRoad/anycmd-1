using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IPageButtonUpdateInput : IEntityUpdateInput
    {
        Guid? FunctionID { get; }
        int IsEnabled { get; }
    }
}

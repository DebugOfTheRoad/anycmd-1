using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IFunctionUpdateInput : IEntityUpdateInput
    {
        string Code { get; }
        bool IsManaged { get; }
        int IsEnabled { get; }
        string Description { get; }
        Guid DeveloperID { get; }
        int SortCode { get; }
    }
}

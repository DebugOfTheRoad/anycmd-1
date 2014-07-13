using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IFunctionCreateInput : IEntityCreateInput
    {
        string Code { get; }
        bool IsManaged { get; }
        int IsEnabled { get; }
        string Description { get; }
        Guid DeveloperID { get; }
        Guid ResourceTypeID { get; }
        int SortCode { get; }
    }
}

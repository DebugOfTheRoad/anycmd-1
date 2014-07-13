using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IDicItemCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string Description { get; }
        Guid DicID { get; }
        int IsEnabled { get; }
        string Name { get; }
        int SortCode { get; }
    }
}

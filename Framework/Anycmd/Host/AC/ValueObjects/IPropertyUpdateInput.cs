using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IPropertyUpdateInput : IEntityUpdateInput
    {
        Guid? ForeignPropertyID { get; }
        string Code { get; }
        string Description { get; }
        Guid? DicID { get; }
        string GuideWords { get; }
        string Icon { get; }
        string InputType { get; }
        bool IsDetailsShow { get; }
        bool IsDeveloperOnly { get; }
        bool IsInput { get; }
        bool IsTotalLine { get; }
        int? MaxLength { get; }
        string Name { get; }
        int SortCode { get; }
    }
}

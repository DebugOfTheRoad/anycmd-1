using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IElementUpdateInput : IEntityUpdateInput
    {
        bool Nullable { get; }
        bool AllowFilter { get; }
        bool AllowSort { get; }
        string Code { get; }
        string Description { get; }
        string FieldCode { get; }
        Guid? GroupID { get; }
        string Icon { get; }
        Guid? InfoDicID { get; }
        int? InputHeight { get; }
        string InputType { get; }
        int? InputWidth { get; }
        bool IsDetailsShow { get; }
        int IsEnabled { get; }
        bool IsExport { get; }
        bool IsGridColumn { get; }
        bool IsImport { get; }
        bool IsInfoIDItem { get; }
        bool IsInput { get; }
        bool IsTotalLine { get; }
        int? MaxLength { get; }
        string Name { get; }
        string Ref { get; }
        string Regex { get; }
        int SortCode { get; }
        int Width { get; }
        string Tooltip { get; }
    }
}

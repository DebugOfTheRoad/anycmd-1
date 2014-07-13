using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IPageButtonCreateInput : IEntityCreateInput
    {
        Guid ButtonID { get; }
        Guid? FunctionID { get; }
        int IsEnabled { get; }
        Guid PageID { get; }
    }
}

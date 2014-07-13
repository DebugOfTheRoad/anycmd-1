using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IResourceCreateInput : IEntityCreateInput
    {
        Guid AppSystemID { get; }
        string Code { get; set; }
        string Description { get; }
        string Icon { get; }
        string Name { get; }
        int SortCode { get; }
    }
}

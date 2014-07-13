using System;

namespace Anycmd.Host.AC.ValueObjects
{
    using Model;

    public interface IEntityTypeCreateInput : IEntityCreateInput
    {
        string Code { get; }
        bool IsOrganizational { get; }
        string Codespace { get; }
        Guid DatabaseID { get; }
        string Description { get; }
        Guid DeveloperID { get; }
        int EditHeight { get; }
        int EditWidth { get; }
        string Name { get; }
        string SchemaName { get; }
        int SortCode { get; }
        string TableName { get; }
    }
}

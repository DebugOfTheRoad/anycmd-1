using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IOntologyUpdateInput : IEntityUpdateInput
    {
        string Code { get; }
        Guid MessageDatabaseID { get; }
        string MessageSchemaName { get; }
        string Description { get; }
        int EditHeight { get; }
        int EditWidth { get; }
        Guid EntityDatabaseID { get; }
        Guid EntityProviderID { get; }
        string EntitySchemaName { get; }
        string EntityTableName { get; }
        string Icon { get; }
        int IsEnabled { get; }
        Guid MessageProviderID { get; }
        string Name { get; }
        int SortCode { get; }
    }
}

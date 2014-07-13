using System;

namespace Anycmd.Host.EDI.ValueObjects
{
    using Model;

    public interface IOntologyCreateInput : IEntityCreateInput
    {
        string Code { get; }
        string Description { get; }
        int EditHeight { get; }
        int EditWidth { get; }
        Guid EntityDatabaseID { get; }
        Guid EntityProviderID { get; }
        string EntitySchemaName { get; }
        string EntityTableName { get; }
        Guid MessageDatabaseID { get; }
        Guid MessageProviderID { get; }
        string MessageSchemaName { get; }
        string Icon { get; }
        int IsEnabled { get; }
        string Name { get; }
        int SortCode { get; }
    }
}

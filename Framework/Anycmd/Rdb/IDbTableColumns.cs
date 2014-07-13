using System.Collections.Generic;
namespace Anycmd.Rdb
{
    public interface IDbTableColumns
    {
        bool TryGetDbTableColumns(RdbDescriptor database, DbTable table, out IReadOnlyDictionary<string, DbTableColumn> dbTableColumns);
        bool TryGetDbTableColumn(RdbDescriptor database, string tableColumnID, out DbTableColumn dbTableColumn);
    }
}

using System.Collections.Generic;

namespace Anycmd.Rdb
{
    public interface IDbViewColumns
    {
        bool TryGetDbViewColumns(RdbDescriptor database, DbView view, out IReadOnlyDictionary<string, DbViewColumn> dbViewColumns);
        bool TryGetDbViewColumn(RdbDescriptor database, string viewColumnID, out DbViewColumn dbViewColumn);
    }
}

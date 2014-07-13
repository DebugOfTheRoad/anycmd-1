using System.Collections.Generic;
namespace Anycmd.Rdb
{
    public interface IDbTables
    {
        IReadOnlyDictionary<string, DbTable> this[RdbDescriptor database] { get; }

        bool TryGetDbTable(RdbDescriptor db, string dbTableID, out DbTable dbTable);
    }
}

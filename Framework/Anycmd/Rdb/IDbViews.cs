using System.Collections.Generic;
namespace Anycmd.Rdb
{
    public interface IDbViews
    {
        IReadOnlyDictionary<string, DbView> this[RdbDescriptor database] { get; }
        bool TryGetDbView(RdbDescriptor db, string dbViewID, out DbView dbView);
    }
}

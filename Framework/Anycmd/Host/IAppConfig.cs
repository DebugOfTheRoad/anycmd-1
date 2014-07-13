using System.Collections.Generic;

namespace Anycmd.Host
{
    public interface IAppConfig
    {
        bool EnableClientCache { get; }
        bool EnableOperationLog { get; }
        IReadOnlyCollection<IParameter> Parameters { get; }
        string SelfAppSystemCode { get; }
        string SqlServerTableColumnsSelect { get; }
        string SqlServerTablesSelect { get; }
        string SqlServerViewColumnsSelect { get; }
        string SqlServerViewsSelect { get; }
        int TicksTimeout { get; }
    }
}


namespace Anycmd.Logging
{
    using Exceptions;
    using log4net;
    using log4net.Config;
    using Query;
    using Rdb;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// <remarks>日志存储在引导库的AnyLog表</remarks>
    /// </summary>
    public sealed class log4netLoggingService : ILoggingService
    {
        /// <summary>
        /// 数据库连接字符串引导库连接字符串
        /// </summary>
        private string _bootConnString = ConfigurationManager.AppSettings["BootDbConnString"];

        /// <summary>
        /// 数据库连接字符串引导库连接字符串
        /// </summary>
        public string BootConnString { get { return _bootConnString; } }

        private readonly IAppHost host;
        private readonly ILog log;

        public log4netLoggingService(IAppHost host)
        {

            log4net.GlobalContext.Properties["ProcessName"] = Process.GetCurrentProcess().ProcessName;
            log4net.GlobalContext.Properties["BaseDirectory"] = AppDomain.CurrentDomain.BaseDirectory;
            this.host = host;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
            log = LogManager.GetLogger(typeof(log4netLoggingService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyLog"></param>
        public void Log(IAnyLog anyLog)
        {
            this.Log(new IAnyLog[] { anyLog });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyLogs"></param>
        public void Log(IAnyLog[] anyLogs)
        {
            // 本组命令类型所对应的数据库表
            string tableID = "[dbo][AnyLog]";
            RdbDescriptor db;
            // TODO:提取配置AnyLog数据库标识
            if (!host.Rdbs.TryDb(new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB"), out db))
            {
                throw new CoreException("意外的AnyLog数据库标识67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
            } 
            DbTable dbTable;
            if (!host.DbTables.TryGetDbTable(db, tableID, out dbTable))
            {
                throw new CoreException("意外的数据库表标识" + tableID);
            }
            // 当前命令表模式克隆得到的新表
            var dt = db.NewTable(dbTable);
            foreach (var log in anyLogs)
            {
                // 将当前命令转化DataRow，一个命令对应一行
                var row = log.ToDataRow(dt);
                dt.Rows.Add(row);
            }

            db.WriteToServer(dt);
        }

        public IAnyLog Get(Guid id)
        {
            RdbDescriptor db;
            // TODO:提取配置AnyLog数据库标识
            if (!host.Rdbs.TryDb(new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB"), out db))
            {
                throw new CoreException("意外的AnyLog数据库标识67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
            }
            string sql = "select * from AnyLog where Id=@Id";
            var reader = db.ExecuteReader(sql, new SqlParameter("Id", id));
            AnyLog anyLog = null;
            if (reader.Read())
            {
                anyLog = AnyLog.Create(reader);
            }
            reader.Close();
            return anyLog;
        }

        public IList<IAnyLog> GetPlistAnyLogs(List<FilterData> filters, PagingInput paging)
        {
            paging.Valid();
            var filterStringBuilder = host.GetRequiredService<ISqlFilterStringBuilder>();
            RdbDescriptor db;
            // TODO:提取配置AnyLog数据库标识
            if (!host.Rdbs.TryDb(new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB"), out db))
            {
                throw new CoreException("意外的AnyLog数据库标识67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
            }
            List<SqlParameter> prams;
            var filterString = filterStringBuilder.FilterString(filters, "t", out prams);
            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = " where " + filterString;
            }
            var sql =
@"select top({0}) * 
from (SELECT ROW_NUMBER() OVER(ORDER BY {1} {2}) AS RowNumber,* FROM {3} as t"
+ filterString + ") a WHERE a.RowNumber > {4}";
            var countSql = "select count(*) from AnyLog as t" + filterString;
            var anyLogs = new List<IAnyLog>();
            var reader = db.ExecuteReader(
                string.Format(sql, paging.pageSize, paging.sortField, paging.sortOrder, "AnyLog", paging.pageSize * paging.pageIndex), prams.ToArray());
            while (reader.Read())
            {
                anyLogs.Add(AnyLog.Create(reader));
            }
            paging.total = (int)db.ExecuteScalar(countSql, prams.Select(p => ((ICloneable)p).Clone()).ToArray());
            reader.Close();

            return anyLogs;
        }

        public IList<OperationLog> GetPlistOperationLogs(Guid? targetID,
            DateTime? leftCreateOn, DateTime? rightCreateOn
            , List<FilterData> filters, PagingInput paging)
        {
            paging.Valid();
            var filterStringBuilder = host.GetRequiredService<ISqlFilterStringBuilder>();
            RdbDescriptor db;
            // TODO:提取配置AnyLog数据库标识
            if (!host.Rdbs.TryDb(new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB"), out db))
            {
                throw new CoreException("意外的AnyLog数据库标识67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
            }
            List<SqlParameter> prams;
            var filterString = filterStringBuilder.FilterString(filters, "t", out prams);
            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = " where " + filterString + "{0}";
            }
            else
            {
                filterString = " where 1=1 {0}";
            }
            if (targetID.HasValue)
            {
                filterString = string.Format(filterString, " and t.TargetID=@TargetID {0}");
            }
            if (leftCreateOn.HasValue)
            {
                filterString = string.Format(filterString, " and t.CreateOn >= @leftCreateOn");
            }
            if (rightCreateOn.HasValue)
            {
                filterString = string.Format(filterString, " and t.CreateOn < @rightCreateOn");
            }
            else
            {
                filterString = string.Format(filterString, string.Empty);
            }
            var sql =
@"select top({0}) * 
from (SELECT ROW_NUMBER() OVER(ORDER BY {1} {2}) AS RowNumber,* FROM {3} as t"
+ filterString + ") a WHERE a.RowNumber > {4}";
            var countSql = "select count(*) from OperationLog as t" + filterString;

            var operationLogs = new List<OperationLog>();
            if (prams == null)
            {
                prams = new List<SqlParameter>();
            }
            if (targetID.HasValue)
            {
                prams.Add(new SqlParameter("TargetID", targetID.Value));
            }
            if (leftCreateOn.HasValue)
            {
                prams.Add(new SqlParameter("leftCreateOn", leftCreateOn.Value));
            }
            if (rightCreateOn.HasValue)
            {
                prams.Add(new SqlParameter("rightCreateOn", rightCreateOn.Value));
            }
            var reader = db.ExecuteReader(
                string.Format(sql, paging.pageSize, paging.sortField, paging.sortOrder, "OperationLog", paging.pageSize * paging.pageIndex), prams.ToArray());
            while (reader.Read())
            {
                operationLogs.Add(OperationLog.Create(reader));
            }
            paging.total = (int)db.ExecuteScalar(countSql, prams.Select(p => ((ICloneable)p).Clone()).ToArray());
            reader.Close();

            return operationLogs;
        }

        public IList<ExceptionLog> GetPlistExceptionLogs(List<FilterData> filters, PagingInput paging)
        {
            paging.Valid();
            var filterStringBuilder = host.GetRequiredService<ISqlFilterStringBuilder>();
            RdbDescriptor db;
            // TODO:提取配置ExceptionLog数据库标识
            if (!host.Rdbs.TryDb(new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB"), out db))
            {
                throw new CoreException("意外的ExceptionLog数据库标识67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
            }
            List<SqlParameter> prams;
            var filterString = filterStringBuilder.FilterString(filters, "t", out prams);
            if (!string.IsNullOrEmpty(filterString))
            {
                filterString = " where " + filterString;
            }
            var sql =
@"select top({0}) * 
from (SELECT ROW_NUMBER() OVER(ORDER BY {1} {2}) AS RowNumber,* FROM {3} as t"
+ filterString + ") a WHERE a.RowNumber > {4}";
            var countSql = "select count(*) from ExceptionLog as t" + filterString;
            var exceptionLogs = new List<ExceptionLog>();
            var reader = db.ExecuteReader(
                string.Format(sql, paging.pageSize, paging.sortField, paging.sortOrder, "ExceptionLog", paging.pageSize * paging.pageIndex), prams.ToArray());
            while (reader.Read())
            {
                exceptionLogs.Add(ExceptionLog.Create(reader));
            }
            paging.total = (int)db.ExecuteScalar(countSql, prams.Select(p => ((ICloneable)p).Clone()).ToArray());
            reader.Close();

            return exceptionLogs;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearAnyLog()
        {
            var sql = "TRUNCATE TABLE dbo.AnyLog";
            using (var conn = new SqlConnection(this.BootConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearExceptionLog()
        {
            var sql = "TRUNCATE TABLE dbo.ExceptionLog";
            using (var conn = new SqlConnection(this.BootConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.ExecuteNonQuery();
            }
        }

        public void Debug(object message)
        {
            log.Debug(message);
        }

        public void DebugFormatted(string format, params object[] args)
        {
            log.DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Info(object message)
        {
            log.Info(message);
        }

        public void InfoFormatted(string format, params object[] args)
        {
            log.InfoFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Warn(object message)
        {
            log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log.Warn(message, exception);
        }

        public void WarnFormatted(string format, params object[] args)
        {
            log.WarnFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Error(object message)
        {
            log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log.Error(message, exception);
        }

        public void ErrorFormatted(string format, params object[] args)
        {
            log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void Fatal(object message)
        {
            log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log.Fatal(message, exception);
        }

        public void FatalFormatted(string format, params object[] args)
        {
            log.FatalFormat(CultureInfo.InvariantCulture, format, args);
        }

        public bool IsDebugEnabled
        {
            get
            {
                return log.IsDebugEnabled;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return log.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return log.IsWarnEnabled;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return log.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return log.IsFatalEnabled;
            }
        }
    }
}

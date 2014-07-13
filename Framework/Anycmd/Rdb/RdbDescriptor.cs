
namespace Anycmd.Rdb
{
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Util;

    /// <summary>
    /// SQLServer数据库模型
    /// </summary>
    public sealed class RdbDescriptor
    {
        private static readonly object globalLocker = new object();

        private readonly Dictionary<string, DataTable> _tableSchemas = new Dictionary<string, DataTable>(StringComparer.OrdinalIgnoreCase);

        private string connString;
        private DbProviderFactory dbProviderFactory = null;
        private string dataSource = null;
        private bool isLocalhost = false;
        private bool isLocalhostDetected = false;
        private object thisLocker = new object();
        private readonly AppHost host;

        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        public RdbDescriptor(AppHost host, IRDatabase database)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }
            this.host = host;
            RdbmsType rdbmsType;
            if (!database.RdbmsType.TryParse(out rdbmsType))
            {
                throw new CoreException("意外的关系数据库类型" + database.RdbmsType);
            }
            this.Database = database;
        }
        #endregion

        #region Public Properties
        public AppHost AppHost { get { return host; } }

        /// <summary>
        /// 
        /// </summary>
        public IRDatabase Database { get; private set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnString
        {
            get
            {
                if (string.IsNullOrEmpty(connString))
                {
                    connString = string.Format(
@"data source={0};initial catalog={1};user id={2};password={3};{4}"
                        , Database.DataSource, Database.CatalogName
                        , Database.UserID, Database.Password, Database.Profile);
                }

                return connString;
            }
        }

        /// <summary>
        /// 查看本数据库的数据源是否指向本机。返回True表示指向本机。
        /// <remarks>
        /// 如果本数据的DataSource属性值为“.”或“localhost”或以“.\”或“localhost\”
        /// 开头则直接返回True。否则通过Dns系统检测是否是本机。
        /// </remarks>
        /// </summary>
        public bool IsLocalhost
        {
            get
            {
                if (!isLocalhostDetected || !string.Equals(dataSource, Database.DataSource, StringComparison.OrdinalIgnoreCase))
                {
                    lock (thisLocker)
                    {
                        isLocalhostDetected = true;
                        dataSource = Database.DataSource;
                        if (string.IsNullOrEmpty(dataSource))
                        {
                            throw new CoreException("数据源为空");
                        }
                        if (dataSource == "."
                            || dataSource.Equals("localhost", StringComparison.OrdinalIgnoreCase)
                            || dataSource.StartsWith(".\\")
                            || dataSource.StartsWith("localhost\\", StringComparison.OrdinalIgnoreCase)
                            || dataSource == IPAddress.Loopback.ToString())
                        {
                            isLocalhost = true;
                        }
                        else
                        {
                            HashSet<string> ips = IPHelper.GetLocalIPs();
                            isLocalhost = ips.Contains(dataSource);
                        }
                    }
                }
                return isLocalhost;
            }
        }
        #endregion

        #region Public Methods

        #region GetConnection
        /// <summary>
        /// Returns a new instance of the provider's class that implements the System.Data.Common.DbConnection
        ///     class.
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            if (dbProviderFactory == null)
            {
                dbProviderFactory = DbProviderFactories.GetFactory(Database.ProviderName);
            }
            var conn = dbProviderFactory.CreateConnection();
            conn.ConnectionString = ConnString;

            return conn;
        }
        #endregion

        #region NewTable
        /// <summary>
        /// 返回给定的表模式克隆得到的新表
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        public DataTable NewTable(DbTable dbTable)
        {
            return this.GetTableSchema(dbTable).Clone();
        }
        #endregion

        #region WriteToServer
        /// <summary>
        /// Copies all rows in the supplied System.Data.DataTable to a destination table
        ///     specified by the System.Data.SqlClient.SqlBulkCopy.DestinationTableName property
        ///     of the System.Data.SqlClient.SqlBulkCopy object.
        /// </summary>
        /// <param name="table">A System.Data.DataTable whose rows will be copied to the destination table.</param>
        public void WriteToServer(DataTable table)
        {
            using (var conn = GetConnection())
            {
                var bulkCopy = new SqlBulkCopy(conn as SqlConnection);
                bulkCopy.DestinationTableName = table.TableName;
                bulkCopy.BatchSize = table.Rows.Count;
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                bulkCopy.WriteToServer(table);
            }
        }
        #endregion

        #region ExecuteNonQuery
        /// <summary>
        /// Executes a SQLText statement against GetConnection().
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sqlText, params object[] parameters)
        {
            return ExecuteNonQuery(sqlText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes a SQL statement against GetConnection().
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sqlText, CommandType commandType, params object[] parameters)
        {
            using (var conn = GetConnection())
            {
                DbCommand cmd = conn.CreateCommand();
                cmd.CommandType = commandType;
                cmd.CommandText = sqlText;
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                return cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region ExecuteReader
        /// <summary>
        /// <remarks>在执行该命令时，如果关闭关联的 DataReader 对象，则关联的 Connection 对象也将关闭。</remarks>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sqlText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlText, params object[] parameters)
        {
            return ExecuteReader(sqlText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the System.Data.Common.DbCommand.CommandText against the System.Data.Common.DbCommand.Connection,
        ///     and returns an System.Data.Common.DbDataReader using one of the System.Data.CommandBehavior
        /// </summary>
        /// <remarks>CommandBehavior.CloseConnection关闭Reader则自动关闭连接</remarks>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DbDataReader ExecuteReader(string sqlText, CommandType commandType, params object[] parameters)
        {
            var conn = GetConnection();
            DbCommand cmd = conn.CreateCommand();
            cmd.CommandText = sqlText;
            cmd.CommandType = commandType;
            if (parameters != null && parameters.Length > 0)
            {
                foreach (var item in parameters)
                {
                    cmd.Parameters.Add(item);
                }
            }
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.Open();
            }
            DbDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            return reader;
        }
        #endregion

        #region ExecuteScalar
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlText, params object[] parameters)
        {
            return ExecuteScalar(sqlText, CommandType.Text, parameters);
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。所有其他的列和行将被忽略。
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sqlText, CommandType commandType, params object[] parameters)
        {
            using (var conn = GetConnection())
            {
                DbCommand cmd = conn.CreateCommand();
                cmd.CommandText = sqlText;
                cmd.CommandType = commandType;
                if (parameters != null && parameters.Length > 0)
                {
                    foreach (var item in parameters)
                    {
                        cmd.Parameters.Add(item);
                    }
                }
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                var result = cmd.ExecuteScalar();

                return result;
            }
        }
        #endregion

        #region Create
        /// <summary>
        /// 创建数据库
        /// <param name="useDb">使用这个数据库创建本数据库</param>
        /// <param name="filePath">将本数据库创建到磁盘的路径</param>
        /// </summary>
        public void Create(RdbDescriptor useDb, string filePath = null)
        {
            using (var conn = useDb.GetConnection())
            {
                var cmd = conn.CreateCommand();
                string config = string.Empty;
                if (!string.IsNullOrEmpty(filePath))
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }
                    string fileName = Path.Combine(filePath, Database.CatalogName + ".mdf");
                    config =
@" ON PRIMARY ( NAME = N'" + Database.CatalogName + @"', FILENAME = N'" + fileName + "')";
                }
                cmd.CommandText =
@"if DB_ID('" + Database.CatalogName + "') IS NULL CREATE DATABASE " + Database.CatalogName + config;
                cmd.CommandType = CommandType.Text;
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var o = obj as RdbDescriptor;
            if (o == null)
            {
                return false;
            }

            return Database == o.Database || Database.Id == o.Database.Id;
        }

        public override int GetHashCode()
        {
            return Database.Id.GetHashCode();
        }
        #endregion

        #region GetTableSchema
        /// <summary>
        /// 获取给定表的表模式
        /// <remarks>表模式是一个ADO.NET表<see cref="DataTable"/></remarks>
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        private DataTable GetTableSchema(DbTable dbTable)
        {
            if (dbTable == null)
            {
                throw new ArgumentNullException("dbTable");
            }
            if (!_tableSchemas.ContainsKey(dbTable.Id))
            {
                lock (globalLocker)
                {
                    if (!_tableSchemas.ContainsKey(dbTable.Id))
                    {
                        IReadOnlyDictionary<string, DbTableColumn> dbTableColumns;
                        if (!host.DbTableColumns.TryGetDbTableColumns(this, dbTable, out dbTableColumns))
                        {
                            throw new CoreException("意外的数据库表");
                        }
                        var dataTable = new DataTable(dbTable.Name);
                        foreach (var col in dbTableColumns.Select(a => a.Value).OrderBy(a => a.Ordinal))
                        {
                            dataTable.Columns.Add(col.ToDataColumn());
                        }
                        _tableSchemas.Add(dbTable.Id, dataTable);
                    }
                }
            }
            return _tableSchemas[dbTable.Id];
        }
        #endregion
    }
}

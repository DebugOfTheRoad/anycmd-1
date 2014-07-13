﻿
namespace Anycmd.Rdb
{
    using Model;

    /// <summary>
    /// 微软SqlServer数据库实体
    /// </summary>
    public interface IRDatabase : IEntity
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsTemplate { get; }

        /// <summary>
        /// 类别码
        /// </summary>
        string RdbmsType { get; }

        /// <summary>
        /// 数据库名
        /// </summary>
        string CatalogName { get; }

        /// <summary>
        /// 数据源
        /// </summary>
        string DataSource { get; }

        /// <summary>
        /// 数据库连接用户名
        /// </summary>
        string UserID { get; }

        /// <summary>
        /// 数据库连接用户密码
        /// </summary>
        string Password { get; }

        /// <summary>
        /// 数据库连接属性
        /// </summary>
        string Profile { get; }

        /// <summary>
        /// 数据访问提供程序名称
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// 说明
        /// </summary>
        string Description { get; }
    }
}

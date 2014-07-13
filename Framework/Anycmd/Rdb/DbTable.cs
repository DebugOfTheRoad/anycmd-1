﻿
namespace Anycmd.Rdb
{
    using Extensions;
    using System;
    using System.Data;

    /// <summary>
    /// 数据库表
    /// </summary>
    public sealed class DbTable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseID"></param>
        /// <param name="id"></param>
        /// <param name="catalogName"></param>
        /// <param name="schemaName"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        internal DbTable(Guid databaseID, string id, string catalogName, string schemaName, string name, string description)
        {
            this.DatabaseID = databaseID;
            this.Id = id;
            this.CatalogName = catalogName;
            this.SchemaName = schemaName;
            this.Name = name;
            this.Description = description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseID"></param>
        /// <param name="reader"></param>
        internal DbTable(Guid databaseID, IDataRecord reader)
            : this(databaseID, reader.GetString(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("CatalogName")),
                reader.GetString(reader.GetOrdinal("SchemaName")),
                reader.GetString(reader.GetOrdinal("Name")),
                reader.GetNullableString("Description"))
        {
        }

        /// <summary>
        /// 数据库实体标识
        /// </summary>
        public Guid DatabaseID { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public String Id { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string CatalogName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string SchemaName { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}

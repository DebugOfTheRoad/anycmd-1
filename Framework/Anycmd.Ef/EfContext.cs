﻿
namespace Anycmd.Ef
{
    using System;
    using System.Configuration;
    using Exceptions;

    /// <summary>
    /// Entity Framework实体框架上下文
    /// </summary>
    public static class EfContext
    {
        /// <summary>
        ///     An application-specific implementation of IObjectContextStorage must be setup either thru
        ///     <see cref = "InitStorage" /> or <see cref = "Storage" /> property.
        /// </summary>
        public static IEfContextStorage Storage { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        public static void InitStorage(IEfContextStorage storage)
        {
            if (storage == null)
            {
                throw new System.ArgumentNullException("storage", "storage mechanism was null but must be provided");
            }
            Storage = storage;
        }

        /// <summary>
        ///     Used to get the current NHibernate session associated with a factory key; i.e., the key 
        ///     associated with an NHibernate session factory for a specific database.
        /// </summary>
        public static System.Data.Entity.DbContext CreateDbContext(AppHost host, string efDbContextName)
        {
            if (string.IsNullOrEmpty(efDbContextName))
            {
                throw new ArgumentNullException(efDbContextName, "objectContextKey may not be null or empty");
            }
            System.Data.Entity.DbContext efDbContext;
            // alert:这里有个约定
            Anycmd.Rdb.RdbDescriptor db;
            string databaseKey = efDbContextName + "DatabaseID";
            Guid databaseID;
            if (!Guid.TryParse(ConfigurationManager.AppSettings[databaseKey], out databaseID))
            {
                throw new CoreException("DatabaseID应是Guid格式");
            }
            if (!host.Rdbs.TryDb(databaseID, out db))
            {
                throw new CoreException("意外的" + databaseKey + ":" + databaseID);
            }
            efDbContext = new System.Data.Entity.DbContext(
                string.Format(ConfigurationManager.ConnectionStrings[efDbContextName].ConnectionString, db.ConnString));

            return efDbContext;
        }
    }
}
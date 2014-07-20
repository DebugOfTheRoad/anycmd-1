
namespace Anycmd.Util
{
    using Exceptions;
    using Host;
    using Model;
    using Rdb;
    using System;
    using System.Data.SqlClient;

    public static class EntityTypeExtension
    {
        public static DicReader GetData(this EntityTypeState entityType, Guid id)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }
            if (entityType == EntityTypeState.Empty || entityType.AppHost == null)
            {
                throw new InvalidOperationException();
            }
            RdbDescriptor db;
            if (!entityType.AppHost.Rdbs.TryDb(entityType.DatabaseID, out db))
            {
                throw new CoreException("意外的实体类型数据库标识" + entityType.Code);
            }
            if (string.IsNullOrEmpty(entityType.TableName))
            {
                throw new CoreException(entityType.Name + "未配置对应的数据库表");
            }
            var sql = "select * from " + string.Format("[{0}]", entityType.TableName) + " as a where Id=@Id";
            using (var reader = db.ExecuteReader(sql, new SqlParameter("Id", id)))
            {
                if (reader.Read())
                {
                    var dic = new DicReader(entityType.AppHost);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dic.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    return dic;
                }
            }
            return null;
        }
    }
}


namespace Anycmd.Host.AC.MessageHandlers
{
    using Anycmd.Rdb;
    using Events;
    using Exceptions;
    using Logging;
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    /// <summary>
    /// 操作事件处理程序
    /// </summary>
    public class OperatedEventHandler : IDomainEventHandler<OperatedEvent>
    {
        Guid operationLogDbID = new Guid("67E6CBF4-B481-4DDD-9FD9-1F0E06E9E1CB");
        private readonly AppHost host;

        public OperatedEventHandler(AppHost host)
        {
            this.host = host;
        }


        public void Handle(OperatedEvent evnt)
        {
            if (evnt == null)
            {
                return;
            }
            var log = evnt.Source as OperationLog;
            if (log != null)
            {
                if (log.TargetID == Guid.Empty)
                {
                    return;
                }
                RdbDescriptor db;
                if (!host.Rdbs.TryDb(operationLogDbID, out db))
                {
                    throw new CoreException("意外的数据库标识");
                }
                // TODO:logbuffer
                var sql = @"INSERT INTO dbo.OperationLog
                                    ( Id ,
                                      FunctionID ,
                                      AccountID ,
                                      EntityTypeID ,
                                      EntityTypeName ,
                                      AppSystemID ,
                                      AppSystemName ,
                                      ResourceTypeID ,
                                      ResourceName ,
                                      Description ,
                                      LoginName ,
                                      UserName ,
                                      CreateOn ,
                                      TargetID ,
                                      IPAddress
                                    )
                            VALUES  ( @Id ,
                                      @FunctionID ,
                                      @AccountID ,
                                      @EntityTypeID ,
                                      @EntityTypeName ,
                                      @AppSystemID ,
                                      @AppSystemName ,
                                      @ResourceTypeID ,
                                      @ResourceName ,
                                      @Description ,
                                      @LoginName ,
                                      @UserName ,
                                      @CreateOn ,
                                      @TargetID ,
                                      @IPAddress
                                    )";
                List<SqlParameter> ps = new List<SqlParameter>();
                if (log.Id == Guid.Empty)
                {
                    log.Id = Guid.NewGuid();
                }
                ps.Add(new SqlParameter("Id", log.Id));
                ps.Add(new SqlParameter("FunctionID", log.FunctionID));
                ps.Add(new SqlParameter("AccountID", log.AccountID));
                ps.Add(new SqlParameter("EntityTypeID", log.EntityTypeID));
                if (log.EntityTypeName == null)
                {
                    ps.Add(new SqlParameter("EntityTypeName", DBNull.Value));
                }
                else
                {
                    ps.Add(new SqlParameter("EntityTypeName", log.EntityTypeName));
                }
                ps.Add(new SqlParameter("AppSystemID", log.AppSystemID));
                if (log.AppSystemName == null)
                {
                    ps.Add(new SqlParameter("AppSystemName", DBNull.Value));
                }
                else
                {
                    ps.Add(new SqlParameter("AppSystemName", log.AppSystemName));
                }
                ps.Add(new SqlParameter("ResourceTypeID", log.ResourceTypeID));
                if (log.ResourceName == null)
                {
                    ps.Add(new SqlParameter("ResourceName", DBNull.Value));
                }
                else
                {
                    ps.Add(new SqlParameter("ResourceName", log.ResourceName));
                }
                if (log.Description == null)
                {
                    ps.Add(new SqlParameter("Description", DBNull.Value));
                }
                else
                {
                    ps.Add(new SqlParameter("Description", log.Description));
                }
                if (log.LoginName == null)
                {
                    ps.Add(new SqlParameter("LoginName", DBNull.Value));
                }
                else
                {
                    ps.Add(new SqlParameter("LoginName", log.LoginName));
                }
                if (log.UserName == null)
                {
                    ps.Add(new SqlParameter("UserName", DBNull.Value));
                }
                else
                {
                    ps.Add(new SqlParameter("UserName", log.UserName));
                }
                ps.Add(new SqlParameter("CreateOn", log.CreateOn));
                ps.Add(new SqlParameter("TargetID", log.TargetID));
                if (log.IPAddress == null)
                {
                    ps.Add(new SqlParameter("IPAddress", DBNull.Value));
                }
                else
                {
                    ps.Add(new SqlParameter("IPAddress", log.IPAddress));
                }
                db.ExecuteNonQuery(sql, ps.ToArray());
            }
        }
    }
}

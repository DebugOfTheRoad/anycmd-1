
namespace Anycmd.Web
{
    using Logging;
    using System;
    using Host;
    using System.Web;

    /// <summary>
    /// 操作日志上下文数据存储在Http上下文中，它的存储位置表明了它的隔离级别为请求级
    /// </summary>
    public sealed class WebOperationLogStorage : IOperationLogStorage
    {
        /// <summary>
        /// 
        /// </summary>
        public void Set(FunctionDescriptor function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            var log = new OperationLog()
            {
                AccountID = function.AccountID,
                CreateOn = function.CreateOn,
                IPAddress = function.IPAddress,
                LoginName = function.LoginName,
                EntityTypeID = function.EntityTypeID,
                EntityTypeName = function.EntityTypeName,
                ResourceTypeID = function.ResourceTypeID,
                ResourceName = function.ResourceName,
                FunctionID = function.FunctionID,
                Description = function.FunctionDescription,
                UserName = function.UserName,
                AppSystemID = function.AppSystemID,
                AppSystemName = function.AppSystemName,
                Id = Guid.NewGuid()
            };
            if (!HttpContext.Current.Items.Contains(ConstKeys.WEB_OPERATIONLOG_OPERATEDE_VENT))
            {
                HttpContext.Current.Items.Add(ConstKeys.WEB_OPERATIONLOG_OPERATEDE_VENT, log);
            }
            else
            {
                HttpContext.Current.Items[ConstKeys.WEB_OPERATIONLOG_OPERATEDE_VENT] = log;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public OperationLogBase Get()
        {
            return HttpContext.Current.Items[ConstKeys.WEB_OPERATIONLOG_OPERATEDE_VENT] as OperationLogBase;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            HttpContext.Current.Items.Remove(ConstKeys.WEB_OPERATIONLOG_OPERATEDE_VENT);
        }
    }
}

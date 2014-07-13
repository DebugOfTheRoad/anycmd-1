
namespace Anycmd.Logging
{
    using Extensions;
    using System;
    using System.Data;
    using System.Diagnostics;
    using Util;

    public class OperationLog : OperationLogBase
    {
        public static OperationLog Create(IDataRecord reader)
        {
            return new OperationLog
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                AccountID = reader.GetGuid(reader.GetOrdinal("AccountID")),
                FunctionID = reader.GetGuid(reader.GetOrdinal("FunctionID")),
                EntityTypeID = reader.GetGuid(reader.GetOrdinal("EntityTypeID")),
                EntityTypeName = reader.GetNullableString("EntityTypeName"),
                AppSystemID = reader.GetGuid(reader.GetOrdinal("AppSystemID")),
                AppSystemName = reader.GetNullableString("AppSystemName"),
                ResourceTypeID = reader.GetGuid(reader.GetOrdinal("ResourceTypeID")),
                ResourceName = reader.GetNullableString("ResourceName"),
                Description = reader.GetNullableString("Description"),
                LoginName = reader.GetNullableString("LoginName"),
                UserName = reader.GetNullableString("UserName"),
                CreateOn = reader.GetDateTime(reader.GetOrdinal("CreateOn")),
                IPAddress = reader.GetNullableString("IPAddress"),
                TargetID = reader.GetGuid(reader.GetOrdinal("TargetID"))
            };
        }

        /// <summary>
        /// 谁
        /// </summary>
        public virtual Guid AccountID { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public virtual string LoginName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// 在什么时间
        /// </summary>
        public virtual DateTime CreateOn { get; set; }

        /// <summary>
        /// 对谁
        /// </summary>
        public virtual Guid FunctionID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 对谁
        /// </summary>
        public virtual Guid EntityTypeID { get; set; }

        /// <summary>
        /// 模型名
        /// </summary>
        public virtual string EntityTypeName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid AppSystemID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string AppSystemName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid ResourceTypeID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ResourceName { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public virtual string IPAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string FromNowString
        {
            get
            {
                return GetFromToString(this.CreateOn, SystemTime.Now());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static string GetFromToString(DateTime from, DateTime? to)
        {
            if (!to.HasValue)
            {
                return "未知";
            }
            TimeSpan span = to.Value - from;
            if (span.TotalDays > 60)
            {
                return from.ToShortDateString();
            }
            else if (span.TotalDays > 30)
            {
                return "1个月前";
            }
            else if (span.TotalDays > 14)
            {
                return "2周前";
            }
            else if (span.TotalDays > 7)
            {
                return "1周前";
            }
            else if (span.TotalDays > 1)
            {
                return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
            }
            else if (span.TotalHours > 1)
            {
                return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 1)
            {
                return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
            }
            else if (span.TotalSeconds >= 1)
            {
                return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
            }
            else
            {
                return "1秒前";
            }
        }
    }
}

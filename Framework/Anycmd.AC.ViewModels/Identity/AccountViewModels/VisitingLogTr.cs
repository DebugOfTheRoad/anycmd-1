using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Anycmd.AC.Identity.ViewModels.AccountViewModels
{
    using Util;

    public class VisitingLogTr : Dictionary<string, object>
    {
        private VisitingLogTr() { }

        public VisitingLogTr(Dictionary<string, object> dic)
        {
            if (dic == null)
            {
                throw new ArgumentNullException("dic");
            }
            foreach (var item in dic)
            {
                this.Add(item.Key, item.Value);
            }
            if (!this.ContainsKey("FromNowString"))
            {
                this.Add("FromNowString", FromNowString);
            }
            if (!this.ContainsKey("TimeSpanString"))
            {
                this.Add("TimeSpanString", TimeSpanString);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string FromNowString
        {
            get
            {
                return GetFromToString((DateTime)this["VisitOn"], SystemTime.Now());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string TimeSpanString
        {
            get
            {
                DateTime? visitedOn;
                if (this["VisitedOn"] == DBNull.Value)
                {
                    visitedOn = null;
                }
                else
                {
                    visitedOn = (DateTime)this["VisitedOn"];
                }
                return GetTimeSpanString((DateTime)this["VisitOn"], visitedOn);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        private static string GetTimeSpanString(DateTime from, DateTime? to)
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
                return "1个月";
            }
            else if (span.TotalDays > 14)
            {
                return "2周";
            }
            else if (span.TotalDays > 7)
            {
                return "1周";
            }
            else if (span.TotalDays > 1)
            {
                return string.Format("{0}天", (int)Math.Floor(span.TotalDays));
            }
            else if (span.TotalHours > 1)
            {
                return string.Format("{0}小时", (int)Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 1)
            {
                return string.Format("{0}分钟", (int)Math.Floor(span.TotalMinutes));
            }
            else if (span.TotalSeconds >= 1)
            {
                return string.Format("{0}秒", (int)Math.Floor(span.TotalSeconds));
            }
            else
            {
                return "1秒";
            }
        }
    }
}

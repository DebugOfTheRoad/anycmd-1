
namespace Anycmd.Query
{
    using Exceptions;
    using System;

    /// <summary>
    /// 分页输入参数
    /// </summary>
    public class PagingInput
    {
        public PagingInput() { }

        /// <summary>
        /// 构造分页参数
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortOrder">排序方向</param>
        public PagingInput(int pageIndex, int pageSize, string sortField, string sortOrder)
        {
            this.pageIndex = pageIndex;
            this.pageSize = pageSize;
            this.sortField = sortField;
            this.sortOrder = sortOrder;
        }

        /// <summary>
        /// 构造分页参数
        /// </summary>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">页尺寸</param>
        /// <param name="sortField">排序字段</param>
        /// <param name="sortOrder">排序方向</param>
        /// <param name="total">总记录数</param>
        public PagingInput(int pageIndex, int pageSize, string sortField, string sortOrder, int? total)
            : this(pageIndex, pageSize, sortField, sortOrder)
        {
            this.total = total;
        }

        /// <summary>
        /// 页索引。零基索引，即第一页对应0
        /// </summary>
        public int pageIndex { get; set; }

        /// <summary>
        /// 页尺寸
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string sortField { get; set; }

        /// <summary>
        /// 排序方向
        /// </summary>
        public string sortOrder { get; set; }

        /// <summary>
        /// 归档数据有一个特性，就是它的总记录数是不改变的，所以传入total以渐少一次数据库count查询
        /// </summary>
        public Int64? total { get; set; }

        /// <summary>
        /// 查看total字段是否为空或者0
        /// </summary>
        private bool IsTotalNullOrZero { get { return !total.HasValue || total.Value == 0; } }

        /// <summary>
        /// pageSize * pageIndex的计算值
        /// </summary>
        public int SkipCount { get { return pageSize * pageIndex; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="func"></param>
        public void Count(Func<int> func)
        {
            if (this.IsTotalNullOrZero && func != null)
            {
                this.total = func();
            }
        }

        /// <summary>
        /// 判断分页输入参数是否合法
        /// </summary>
        /// <returns></returns>
        public void Valid()
        {
            if (string.IsNullOrEmpty(sortField) || string.IsNullOrEmpty(sortOrder))
            {
                throw new ValidationException("排序是必须的");
            }
            if (sortOrder.ToLower() != "asc" && sortOrder.ToLower() != "desc")
            {
                throw new ValidationException("排序方向只能是asc或desc");
            }
        }
    }
}


namespace Anycmd.ViewModel
{
    using Query;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 分页获取实体集
    /// </summary>
    public class GetPlistResult : PagingInput, IGetPlistResult
    {
        private List<FilterData> _filters;

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public new int? pageIndex
        {
            get
            {
                return base.pageIndex;
            }
            set
            {
                base.pageIndex = !value.HasValue ? 10 : value.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public new int? pageSize
        {
            get
            {
                return base.pageSize;
            }
            set
            {
                base.pageSize = !value.HasValue ? 10 : value.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public new string sortField
        {
            get { return base.sortField; }
            set { base.sortField = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        public new string sortOrder
        {
            get { return base.sortOrder; }
            set { base.sortOrder = value; }
        }

        /// <summary>
        /// 设计用于消除一次sql count查询。如果传入的total参数不为0则数据访问层忽略count查询
        /// </summary>
        public new int? total
        {
            get
            {
                return base.total.HasValue ? (int)base.total.Value : 0;
            }
            set
            {
                if (value.HasValue)
                {
                    base.total = value.Value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<FilterData> filters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = new List<FilterData>();
                }
                return _filters;
            }
            set
            {
                _filters = value;
            }
        }
    }
}


namespace Anycmd.ViewModel
{
    using System.Collections.Generic;
    using Query;
    using Model;

    /// <summary>
    /// 分页获取数据集的输入模型
    /// </summary>
    public interface IGetPlistResult : IInputModel
    {
        /// <summary>
        /// 
        /// </summary>
        int? pageIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int? pageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string sortField { get; set; }
        /// <summary>
        /// 
        /// </summary>
        string sortOrder { get; set; }
        /// <summary>
        /// 设计用于消除一次sql count查询。如果传入的total参数不为0则数据访问层忽略count查询
        /// </summary>
        int? total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        List<FilterData> filters { get; set; }
    }
}

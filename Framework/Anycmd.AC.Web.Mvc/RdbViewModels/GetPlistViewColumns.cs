
namespace Anycmd.RdbViewModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 分页获取数据库视图列
    /// </summary>
    public sealed class GetPlistViewColumns : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid? databaseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string schemaName { get; set; }
        [Required]
        /// <summary>
        /// 
        /// </summary>
        public string viewName { get; set; }
        [Required]
        public string viewID { get; set; }
    }
}

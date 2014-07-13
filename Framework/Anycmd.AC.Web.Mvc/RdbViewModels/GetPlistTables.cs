
namespace Anycmd.RdbViewModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 分页获取数据库表
    /// </summary>
    public sealed class GetPlistTables : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid? databaseID { get; set; }
    }
}

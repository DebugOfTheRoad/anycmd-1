
namespace Anycmd.RdbViewModel
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 分页获取数据库表列
    /// </summary>
    public sealed class GetPlistTableColumns : GetPlistResult
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
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string tableName { get; set; }
        [Required]
        public string tableID { get; set; }
    }
}

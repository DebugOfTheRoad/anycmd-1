
namespace Anycmd.AC.ViewModels.RdbViewModel
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 关系数据库列输入模型
    /// </summary>
    public sealed class DbTableColumnInput
    {
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid DatabaseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Description { get; set; }
    }
}

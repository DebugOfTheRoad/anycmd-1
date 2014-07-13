
namespace Anycmd.RdbViewModel
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 数据库视图列输入模型
    /// </summary>
    public sealed class DbViewColumnInput
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


namespace Anycmd.EDI.MessageViewModels {
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 分页获取命令。
    /// </summary>
    public class GetPlistMessages : GetPlistResult {
        /// <summary>
        /// 命令类型
        /// </summary>
        [Required]
        public string commandType { get; set; }
        /// <summary>
        /// 组织结构码
        /// </summary>
        public string organizationCode { get; set; }
        /// <summary>
        /// 动作码
        /// </summary>
        public string actionCode { get; set; }
        /// <summary>
        /// 节点标识
        /// </summary>
        public Guid? nodeID { get; set; }
        /// <summary>
        /// 本体码
        /// </summary>
        [Required]
        public string ontologyCode { get; set; }
        /// <summary>
        /// 实体标识
        /// </summary>
        public string entityID { get; set; }
    }
}

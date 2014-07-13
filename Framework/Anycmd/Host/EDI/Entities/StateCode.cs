
namespace Anycmd.Host.EDI.Entities
{
    using Model;

    /// <summary>
    /// 状态码文档。该表的Code和ReasonPhrase字段由上下文自动维护，Description由人工维护。
    /// </summary>
    public sealed class StateCode : EntityBase, IAggregateRoot
    {
        public StateCode() { }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 原因短语
        /// </summary>
        public string ReasonPhrase { get; set; }
    }
}

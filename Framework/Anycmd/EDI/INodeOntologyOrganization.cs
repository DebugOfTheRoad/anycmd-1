
namespace Anycmd.EDI {
    using System;

    /// <summary>
    /// <remarks>
    /// 如此简单的模型为什么是接口？使用接口将其约束为不可变模型，从而使插件开发者不能使用正常手段修改它。
    /// </remarks>
    /// </summary>
    public interface INodeOntologyOrganization {
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid NodeID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid OntologyID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid OrganizationID { get; }
        /// <summary>
        /// 
        /// </summary>
        string Actions { get; }
    }
}

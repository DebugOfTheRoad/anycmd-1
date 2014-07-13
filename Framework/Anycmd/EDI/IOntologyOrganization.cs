
namespace Anycmd.EDI {
    using System;

    /// <summary>
    /// 本体组织结构级配置模型
    /// </summary>
    public interface IOntologyOrganization {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
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

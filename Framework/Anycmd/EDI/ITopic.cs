
namespace Anycmd.EDI
{
    using System;

    /// <summary>
    /// // TODO:重命名为Topic，因为User将被重命名为Subject
    /// </summary>
    public interface ITopic
    {
        Guid Id { get; }
        /// <summary>
        /// 本体标识
        /// </summary>
        Guid OntologyID { get; }

        /// <summary>
        /// 动作码
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 动作名
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsAllowed { get; }
        /// <summary>
        /// 
        /// </summary>
        string Description { get; }

        DateTime? CreateOn { get; }
    }
}

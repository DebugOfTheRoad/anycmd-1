
namespace Anycmd.EDI {
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IElementInfoRule {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid ElementID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid InfoRuleID { get; }
        /// <summary>
        /// 
        /// </summary>
        int IsEnabled { get; }
        /// <summary>
        /// 
        /// </summary>
        int SortCode { get; }
        DateTime CreateOn { get; }
    }
}

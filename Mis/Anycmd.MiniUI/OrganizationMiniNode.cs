
namespace Anycmd.MiniUI
{
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationMiniNode : MiniNode
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ParentCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string CategoryCode { get; set; }
    }
}

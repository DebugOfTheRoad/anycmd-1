
namespace Anycmd.MiniUI
{
    using Extensions;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public abstract class MiniNode : IViewModel
    {
        private string id = null;
        /// <summary>
        /// 
        /// </summary>
        public virtual string Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value.SafeToLower();
            }
        }
        private string parentID = null;
        /// <summary>
        /// 
        /// </summary>
        public virtual string ParentID
        {
            get
            {
                return parentID;
            }
            set
            {
                parentID = value.SafeToLower();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool isLeaf { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool expanded { get; set; }
    }
}

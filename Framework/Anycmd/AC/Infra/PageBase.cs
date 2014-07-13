
namespace Anycmd.AC.Infra
{
    using Model;

    /// <summary>
    /// 页面基类<see cref="IPage"/>
    /// </summary>
    public abstract class PageBase : EntityBase, IPage
    {

        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Tooltip { get; set; }
    }
}

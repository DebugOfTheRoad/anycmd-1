
namespace Anycmd.AC.Infra.ViewModels.MenuViewModels
{
    using System;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class MenuMiniNode
    {
        private readonly AppHost host;

        public MenuMiniNode(AppHost host)
        {
            this.host = host;
        }

        public static MenuMiniNode Create(AppHost host, IMenu menu)
        {
            return new MenuMiniNode(host)
            {
                Id = menu.Id,
                expanded = false,
                img = menu.Icon,
                Name = menu.Name,
                ParentID = menu.ParentID,
                SortCode = menu.SortCode,
                Url = menu.Url
            };
        }

        public virtual Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? ParentID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool isLeaf
        {
            get
            {
                return !host.MenuSet.Any(a => a.ParentID == this.Id);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool expanded { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Url { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string img { get; set; }
    }
}

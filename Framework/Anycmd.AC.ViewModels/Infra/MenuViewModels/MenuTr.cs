
namespace Anycmd.AC.Infra.ViewModels.MenuViewModels
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class MenuTr
    {
        public MenuTr() { }

        public static MenuTr Create(IMenu menu)
        {
            return new MenuTr
            {
                Icon = menu.Icon,
                Id = menu.Id,
                Name = menu.Name,
                ParentID = menu.ParentID,
                SortCode = menu.SortCode,
                Url = menu.Url
            };
        }

        /// <summary>
        /// 
        /// </summary>
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
        public virtual string Url { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }
    }
}

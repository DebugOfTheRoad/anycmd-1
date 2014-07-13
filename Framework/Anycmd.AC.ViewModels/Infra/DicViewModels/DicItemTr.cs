
namespace Anycmd.AC.Infra.ViewModels.DicViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class DicItemTr
    {
        public DicItemTr() { }

        public static DicItemTr Create(DicItemState dicItem)
        {
            return new DicItemTr
            {
                Code = dicItem.Code,
                CreateOn = dicItem.CreateOn,
                DicID = dicItem.DicID,
                Id = dicItem.Id,
                IsEnabled = dicItem.IsEnabled,
                Name = dicItem.Name,
                SortCode = dicItem.SortCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DicID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}

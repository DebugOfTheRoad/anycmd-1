
namespace Anycmd.AC.Infra.ViewModels.DicViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class DicTr
    {
        public DicTr() { }

        public static DicTr Create(DicState dic)
        {
            return new DicTr
            {
                Code = dic.Code,
                CreateOn = dic.CreateOn,
                Id = dic.Id,
                IsEnabled = dic.IsEnabled,
                Name = dic.Name,
                SortCode = dic.SortCode
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


namespace Anycmd.AC.ViewModels.GroupViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class GroupTr
    {
        public GroupTr() { }

        public static GroupTr Create(GroupState group)
        {
            return new GroupTr
            {
                Id = group.Id,
                CategoryCode = group.CategoryCode,
                Name = group.Name,
                SortCode = group.SortCode,
                IsEnabled = group.IsEnabled,
                CreateOn = group.CreateOn
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CategoryCode { get; set; }

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

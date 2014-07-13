
namespace Anycmd.AC.Infra.ViewModels.ResourceViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class ResourceTypeTr
    {
        public ResourceTypeTr() { }

        public static ResourceTypeTr Create(ResourceTypeState resource)
        {
            return new ResourceTypeTr
            {
                Code = resource.Code,
                CreateOn = resource.CreateOn,
                Icon = resource.Icon,
                Id = resource.Id,
                Name = resource.Name,
                SortCode = resource.SortCode
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
        public virtual string Icon { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
    }
}

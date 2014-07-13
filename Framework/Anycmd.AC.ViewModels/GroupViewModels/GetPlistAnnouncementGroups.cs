
namespace Anycmd.AC.ViewModels.GroupViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistAnnouncementGroups : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required]
        public Guid? announcementId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? isAssigned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? categoryID { get; set; }
    }
}


namespace Anycmd.AC.ViewModels.RoleViewModels
{
    using System;
    using ViewModel;

    /// <summary>
    /// 
    /// </summary>
    public class GetPlistAnnouncementRoles : GetPlistResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid announcementId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? isAssigned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// 
        /// </summary>
        public Guid? categoryID { get; set; }
    }
}


namespace Anycmd.AC.Infra.ViewModels.FunctionViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class FunctionTr
    {
        private readonly IAppHost host;

        private FunctionTr(IAppHost host)
        {
            this.host = host;
        }

        public static FunctionTr Create(FunctionState function)
        {
            if (function == null)
            {
                return null;
            }
            return new FunctionTr(function.AppHost)
            {
                AppSystemID = function.AppSystem.Id,
                AppSystemCode = function.AppSystem.Code,
                AppSystemName = function.AppSystem.Name,
                Code = function.Code,
                CreateOn = function.CreateOn,
                Description = function.Description,
                DeveloperID = function.DeveloperID,
                Id = function.Id,
                IsManaged = function.IsManaged,
                IsEnabled = function.IsEnabled,
                ResourceCode = function.Resource.Code,
                ResourceTypeID = function.Resource.Id,
                ResourceName = function.Resource.Name,
                SortCode = function.SortCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsManaged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsPage
        {
            get
            {
                PageState page;
                return host.PageSet.TryGetPage(this.Id, out page);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }

        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid ResourceTypeID { get; set; }

        public virtual Guid AppSystemID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string AppSystemCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string AppSystemName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ResourceCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string ResourceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DeveloperID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }

        public string DeveloperCode
        {
            get
            {
                AccountState developer;
                if (!host.SysUsers.TryGetDevAccount(this.DeveloperID, out developer))
                {
                    return "无效的值";
                }
                return developer.LoginName;
            }
        }
    }
}

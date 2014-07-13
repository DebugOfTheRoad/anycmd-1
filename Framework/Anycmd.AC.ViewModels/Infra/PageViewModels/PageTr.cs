
namespace Anycmd.AC.Infra.ViewModels.PageViewModels
{
    using Anycmd.Host;
    using Exceptions;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class PageTr
    {
        private readonly AppHost host;

        private PageTr(AppHost host)
        {
            this.host = host;
        }

        public static PageTr Create(PageState page)
        {
            if (page == null)
            {
                return null;
            }
            FunctionState function;
            page.AppHost.FunctionSet.TryGetFunction(page.Id, out function);
            AppSystemState appSystem;
            page.AppHost.AppSystemSet.TryGetAppSystem(function.AppSystem.Id, out appSystem);
            return new PageTr(page.AppHost)
            {
                Code = function.Code,
                AppSystemCode = appSystem.Code,
                AppSystemID = appSystem.Id,
                AppSystemName = appSystem.Name,
                ResourceCode = function.Resource.Code,
                CreateOn = page.CreateOn,
                DeveloperID = function.DeveloperID,
                Icon = page.Icon,
                Id = page.Id,
                Description = function.Description
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
        public virtual string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string ResourceCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DeveloperID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }

        public virtual string DeveloperCode
        {
            get
            {
                AccountState developer;
                if (!host.SysUsers.TryGetDevAccount(this.DeveloperID, out developer))
                {
                    throw new ValidationException("意外的开发人员标识" + this.DeveloperID);
                }
                return developer.LoginName;
            }
        }
    }
}

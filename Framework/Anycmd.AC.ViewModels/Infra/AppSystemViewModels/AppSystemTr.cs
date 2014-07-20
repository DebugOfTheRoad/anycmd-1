
namespace Anycmd.AC.Infra.ViewModels.AppSystemViewModels
{
    using Anycmd.AC.Identity;
    using Anycmd.Host;
    using Exceptions;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class AppSystemTr
    {
        private AccountState principal;
        private readonly IAppHost host;

        private AppSystemTr(IAppHost host)
        {
            this.host = host;
        }

        public static AppSystemTr Create(IAppHost host, AppSystemState appSystem)
        {
            return new AppSystemTr(host)
            {
                Code = appSystem.Code,
                CreateOn = appSystem.CreateOn,
                Icon = appSystem.Icon,
                Id = appSystem.Id,
                IsEnabled = appSystem.IsEnabled,
                Name = appSystem.Name,
                PrincipalID = appSystem.PrincipalID,
                SortCode = appSystem.SortCode,
                SSOAuthAddress = appSystem.SSOAuthAddress
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }

        /// <summary>
        /// 单点登录Http认证接口地址
        /// </summary>
        public virtual string SSOAuthAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>

        public virtual Guid? PrincipalID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string PrincipalName
        {
            get
            {
                if (this.PrincipalID.HasValue)
                {
                    return this.Principal.LoginName;
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }

        /// <summary>
        /// 字符串类型，枚举值：“禁用”或“正常”
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }

        private IAccount Principal
        {
            get
            {
                if (this.PrincipalID.HasValue)
                {
                    if (!host.SysUsers.TryGetDevAccount(this.PrincipalID.Value, out principal))
                    {
                        throw new ValidationException("意外的开发人员标识" + this.PrincipalID);
                    }
                    return principal;
                }
                return null;
            }
        }
    }
}

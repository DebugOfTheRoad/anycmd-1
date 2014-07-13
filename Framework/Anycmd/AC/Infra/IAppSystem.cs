
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 定义应用系统
    /// </summary>
    public interface IAppSystem
    {
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 
        /// </summary>
        int SortCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Guid PrincipalID { get; set; }

        int IsEnabled { get; }

        string SSOAuthAddress { get; }

        string Icon { get; set; }
    }
}

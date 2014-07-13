
namespace Anycmd.AC.Infra
{
    using System;

    /// <summary>
    /// 系统字段模型接口，设计用于支持自动化界面等。
    /// <remarks>该模型是程序开发模型，被程序员使用，用户不关心本概念。</remarks>
    /// </summary>
    public interface IProperty
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid EntityTypeID { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid? ForeignPropertyID { get; }
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
        string GuideWords { get; }
        /// <summary>
        /// 
        /// </summary>
        string Tooltip { get; set; }
        /// <summary>
        /// 
        /// </summary>
        int? MaxLength { get; }
        /// <summary>
        /// 
        /// </summary>
        int SortCode { get; }
        /// <summary>
        /// 
        /// </summary>
        Guid? DicID { get; }
        /// <summary>
        /// 
        /// </summary>
        string Icon { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsDetailsShow { get; }
        /// <summary>
        /// 是否是开发人员专用字段
        /// </summary>
        bool IsDeveloperOnly { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsInput { get; }
        /// <summary>
        /// 
        /// </summary>
        string InputType { get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsTotalLine { get; }
    }
}

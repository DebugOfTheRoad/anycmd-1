
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using System;
    using ValueObjects;

    /// <summary>
    /// 字段，设计用于支持字段级配置和自动化界面等。
    /// <remarks>该模型是程序开发模型，被程序员使用，最终用户不关心本概念。</remarks>
    /// </summary>
    public class Property : PropertyBase, IAggregateRoot
    {
        public Property() { }

        public static Property Create(IPropertyCreateInput input)
        {
            return new Property
            {
                Id = input.Id.Value,
                Code = input.Code,
                Name = input.Name,
                DicID = input.DicID,
                Description = input.Description,
                EntityTypeID = input.EntityTypeID,
                GroupCode = input.GroupCode,
                ForeignPropertyID = input.ForeignPropertyID,
                Icon = input.Icon,
                GuideWords = input.GuideWords,
                InputType = input.InputType,
                IsDetailsShow = input.IsDetailsShow,
                IsDeveloperOnly = input.IsDeveloperOnly,
                IsInput = input.IsInput,
                IsTotalLine = input.IsTotalLine,
                SortCode = input.SortCode,
                Tooltip = input.Tooltip,
                MaxLength = input.MaxLength,
                CreateOn = DateTime.Now
            };
        }

        public void Update(IPropertyUpdateInput input)
        {
            this.ForeignPropertyID = input.ForeignPropertyID;
            this.Code = input.Code;
            this.DicID = input.DicID;
            this.Description = input.Description;
            this.GuideWords = input.GuideWords;
            this.Icon = input.Icon;
            this.InputType = input.InputType;
            this.IsDetailsShow = input.IsDetailsShow;
            this.IsDeveloperOnly = input.IsDeveloperOnly;
            this.IsInput = input.IsInput;
            this.IsTotalLine = input.IsTotalLine;
            this.MaxLength = input.MaxLength;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
        }
    }
}

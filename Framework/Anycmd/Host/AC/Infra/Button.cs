
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 按钮，设计用于支持自动化界面等。
    /// <remarks>该模型是程序开发模型，被程序员使用，用户不关心本概念。</remarks>
    /// </summary>
    public class Button : ButtonBase, IAggregateRoot
    {
        #region Ctor
        public Button()
        {
        }
        #endregion

        public static Button Create(IButtonCreateInput input)
        {
            return new Button
            {
                Id = input.Id.Value,
                Code = input.Code,
                Name = input.Name,
                Icon = input.Icon,
                Description = input.Description,
                CategoryCode = input.CategoryCode,
                IsEnabled = input.IsEnabled,
                SortCode = input.SortCode
            };
        }

        public void Update(IButtonUpdateInput input)
        {
            this.Code = input.Code;
            this.CategoryCode = input.CategoryCode;
            this.Description = input.Description;
            this.Icon = input.Icon;
            this.IsEnabled = input.IsEnabled;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
        }
    }
}

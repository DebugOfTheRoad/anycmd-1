
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 页面菜单，设计用于支持自动化界面等。将页面和按钮的关系视作实体。
    /// <remarks>
    /// 1该模型是程序开发模型，被程序员使用，用户不关心本概念;
    /// 2在数据库中没有任何表需要引用PageButton表所以PageButton无需标记删除
    /// </remarks>
    /// </summary>
    public class PageButton : PageButtonBase, IAggregateRoot
    {
        #region Ctor
        public PageButton()
        {
            IsEnabled = 1;
        }
        #endregion

        public static PageButton Create(IPageButtonCreateInput input)
        {
            return new PageButton
                {
                    Id = input.Id.Value,
                    ButtonID = input.ButtonID,
                    PageID = input.PageID,
                    FunctionID = input.FunctionID,
                    IsEnabled = input.IsEnabled
                };
        }

        public void Update(IPageButtonUpdateInput input)
        {
            this.IsEnabled = input.IsEnabled;
            this.FunctionID = input.FunctionID;
        }
    }
}

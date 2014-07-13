
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 菜单
    /// <remarks>
    /// <para>该菜单是首页左侧的导航树中树节点的抽象，不是面上的按钮，按钮是Button</para>
    /// </remarks>
    /// </summary>
    public class Menu : MenuBase, IAggregateRoot
    {
        public Menu()
        {
            AllowDelete = 1;
            AllowEdit = 1;
        }

        public static Menu Create(IMenuCreateInput input)
        {
            return new Menu
            {
                AppSystemID = input.AppSystemID,
                Id = input.Id.Value,
                Name = input.Name,
                Icon = input.Icon,
                Description = input.Description,
                ParentID = input.ParentID,
                SortCode = input.SortCode,
                Url = input.Url
            };
        }

        public void Update(IMenuUpdateInput input)
        {
            this.AppSystemID = input.AppSystemID;
            this.Description = input.Description;
            this.Icon = input.Icon;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
            this.Url = input.Url;
        }
    }
}

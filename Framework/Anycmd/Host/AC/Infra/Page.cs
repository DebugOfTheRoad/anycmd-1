
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 系统页面。设计用于支持页面级配置和自动节点等。
    /// </summary>
    public class Page : PageBase, IAggregateRoot
    {
        public Page() { }

        public static Page Create(IPageCreateInput input)
        {
            return new Page
            {
                Id = input.Id.Value,
                Icon = input.Icon,
                Tooltip = input.Tooltip
            };
        }

        public void Update(IPageUpdateInput input)
        {
            this.Tooltip = input.Tooltip;
            this.Icon = input.Icon;
        }
    }
}

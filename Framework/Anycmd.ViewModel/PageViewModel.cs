
namespace Anycmd.ViewModel
{
    using Anycmd.Host;
    using System;

    public class PageViewModel
    {
        public static readonly PageViewModel Empty = new PageViewModel(PageState.Empty, "无名页面");

        public PageViewModel(PageState page, string title)
        {
            this.Page = page;
            this.Id = page.Id;
            this.Tooltip = page.Tooltip;
            this.Icon = page.Icon;
            this.Title = title;
        }

        public PageState Page { get; private set; }

        public Guid Id { get; private set; }

        public string Title { get; private set; }

        public string Tooltip { get; private set; }

        public string Icon { get; private set; }
    }
}

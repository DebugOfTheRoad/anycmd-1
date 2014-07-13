
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using System;
    using Util;

    public sealed class PageState : IPage
    {
        public static readonly PageState Empty = new PageState
        {
            CreateOn = SystemTime.MinDate,
            Icon = string.Empty,
            Id = Guid.Empty,
            Tooltip = string.Empty
        };

        private PageState() { }

        public static PageState Create(AppHost host, PageBase page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }

            return new PageState
            {
                AppHost = host,
                Id = page.Id,
                Tooltip = page.Tooltip,
                CreateOn = page.CreateOn,
                Icon = page.Icon,
            };
        }

        public AppHost AppHost { get; private set; }

        public Guid Id { get; private set; }

        public string Tooltip { get; set; }

        public string Icon { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!(obj is PageState))
            {
                return false;
            }
            var left = this;
            var right = (PageState)obj;
            return left.Id == right.Id &&
                left.Tooltip == right.Tooltip &&
                left.Icon == right.Icon;
        }

        public static bool operator ==(PageState a, PageState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(PageState a, PageState b)
        {
            return !(a == b);
        }
    }
}


namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using Exceptions;
    using System;

    public sealed class PageButtonState : IPageButton
    {
        private Guid _pageID;
        private Guid _buttonID;
        private Guid? _functionID;
        private IAppHost AppHost { get; set; }

        private PageButtonState() { }

        public static PageButtonState Create(IAppHost host, PageButtonBase pageButton)
        {
            if (pageButton == null)
            {
                throw new ArgumentNullException("pageButton");
            }
            PageState page;
            if (!host.PageSet.TryGetPage(pageButton.PageID, out page))
            {
                throw new CoreException("意外的页面" + pageButton.PageID);
            }
            ButtonState button;
            if (!host.ButtonSet.TryGetButton(pageButton.ButtonID, out button))
            {
                throw new CoreException("意外的按钮" + pageButton.ButtonID);
            }
            return new PageButtonState
            {
                AppHost = host,
                Id = pageButton.Id,
                PageID = pageButton.PageID,
                FunctionID = pageButton.FunctionID,
                ButtonID = pageButton.ButtonID,
                IsEnabled = pageButton.IsEnabled,
                CreateOn = pageButton.CreateOn
            };
        }

        public Guid Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid PageID
        {
            get { return _pageID; }
            private set
            {
                PageState page;
                if (!AppHost.PageSet.TryGetPage(value, out page))
                {
                    throw new ValidationException("以外的页面标识" + value);
                }
                _pageID = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Guid ButtonID
        {
            get { return _buttonID; }
            set
            {
                ButtonState button;
                if (!AppHost.ButtonSet.TryGetButton(value, out button))
                {
                    throw new ValidationException("意外的按钮标识" + value);
                }
                _buttonID = value;
            }
        }

        public Guid? FunctionID
        {
            get { return _functionID; }
            private set
            {
                if (value == Guid.Empty)
                {
                    value = null;
                }
                if (value.HasValue)
                {
                    FunctionState function;
                    if (!AppHost.FunctionSet.TryGetFunction(value.Value, out function))
                    {
                        throw new ValidationException("意外的功能标识" + value);
                    }
                    _functionID = value;
                }
            }
        }

        public int IsEnabled { get; private set; }

        public DateTime? CreateOn { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ButtonState Button
        {
            get
            {
                ButtonState button;
                if (!AppHost.ButtonSet.TryGetButton(this.ButtonID, out button))
                {
                    throw new CoreException("意外的按钮" + this.ButtonID);
                }
                return button;
            }
        }

        public PageState Page
        {
            get
            {
                PageState page;
                if (!AppHost.PageSet.TryGetPage(this.PageID, out page))
                {
                    throw new CoreException("意外的页面按钮页面标识" + this.PageID);
                }
                return page;
            }
        }

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
            if (!(obj is PageButtonState))
            {
                return false;
            }
            var left = this;
            var right = (PageButtonState)obj;

            return left.Id == right.Id &&
                left.PageID == right.PageID &&
                left.FunctionID == right.FunctionID &&
                left.ButtonID == right.ButtonID &&
                left.IsEnabled == right.IsEnabled;
        }

        public static bool operator ==(PageButtonState a, PageButtonState b)
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

        public static bool operator !=(PageButtonState a, PageButtonState b)
        {
            return !(a == b);
        }
    }
}

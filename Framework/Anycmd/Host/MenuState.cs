
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using Exceptions;
    using System;

    public sealed class MenuState : IMenu
    {
        private IAppHost AppHost { get; set; }
        private Guid _appSystemID;

        private MenuState() { }

        public static MenuState Create(IAppHost host, IMenu menu)
        {
            if (menu == null)
            {
                throw new ArgumentNullException("menu");
            }
            if (!host.AppSystemSet.ContainsAppSystem(menu.AppSystemID))
            {
                throw new ValidationException("意外的应用系统标识" + menu.AppSystemID);
            }
            return new MenuState
            {
                AppHost = host,
                Id = menu.Id,
                AppSystemID = menu.AppSystemID,
                Name = menu.Name,
                ParentID = menu.ParentID,
                Url = menu.Url,
                Icon = menu.Icon,
                SortCode = menu.SortCode,
                AllowDelete = menu.AllowDelete,
                AllowEdit = menu.AllowEdit,
                Description = menu.Description
            };
        }

        public Guid Id { get; private set; }

        public Guid AppSystemID
        {
            get { return _appSystemID; }
            private set
            {
                if (!AppHost.AppSystemSet.ContainsAppSystem(value))
                {
                    throw new ValidationException("意外的功能应用系统标识" + value);
                }
                _appSystemID = value;
            }
        }

        public Guid? ParentID { get; private set; }

        public string Name { get; private set; }

        public string Url { get; private set; }

        public string Icon { get; private set; }

        public int SortCode { get; private set; }

        public int? AllowEdit { get; private set; }
        public int? AllowDelete { get; private set; }

        public string Description { get; private set; }

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
            if (!(obj is MenuState))
            {
                return false;
            }
            var left = this;
            var right = (MenuState)obj;

            return left.Id == right.Id &&
                left.AppSystemID == right.AppSystemID &&
                left.ParentID == right.ParentID &&
                left.Name == right.Name &&
                left.Url == right.Url &&
                left.Icon == right.Icon &&
                left.SortCode == right.SortCode &&
                left.AllowDelete == right.AllowDelete &&
                left.AllowEdit == right.AllowEdit &&
                left.Description == right.Description;
        }

        public static bool operator ==(MenuState a, MenuState b)
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

        public static bool operator !=(MenuState a, MenuState b)
        {
            return !(a == b);
        }
    }
}


namespace Anycmd.Host
{
    using Anycmd.AC;
    using System;

    public sealed class RoleState : IRole
    {
        private RoleState() { }

        public static RoleState Create(RoleBase role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            return new RoleState
            {
                Id = role.Id,
                Name = role.Name,
                CategoryCode = role.CategoryCode,
                CreateOn = role.CreateOn,
                IsEnabled = role.IsEnabled,
                Icon = role.Icon,
                SortCode = role.SortCode
            };
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string CategoryCode { get; private set; }
        public DateTime? CreateOn { get; private set; }
        public int IsEnabled { get; private set; }
        public string Icon { get; private set; }
        public int SortCode { get; private set; }

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
            if (!(obj is RoleState))
            {
                return false;
            }
            var left = this;
            var right = (RoleState)obj;

            return left.Id == right.Id &&
                left.Name == right.Name &&
                left.CategoryCode == right.CategoryCode &&
                left.IsEnabled == right.IsEnabled &&
                left.Icon == right.Icon &&
                left.SortCode == right.SortCode;
        }

        public static bool operator ==(RoleState a, RoleState b)
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

        public static bool operator !=(RoleState a, RoleState b)
        {
            return !(a == b);
        }
    }
}

using System;

namespace Anycmd.Host
{
    using Anycmd.AC;

    public sealed class GroupState : IGroup
    {
        private GroupState() { }

        public static GroupState Create(GroupBase group)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            return new GroupState
            {
                Id = group.Id,
                Name = group.Name,
                OrganizationCode = group.OrganizationCode,
                CategoryCode = group.CategoryCode,
                SortCode = group.SortCode,
                IsEnabled = group.IsEnabled,
                CreateOn = group.CreateOn
            };
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string OrganizationCode { get; private set; }

        public string CategoryCode { get; private set; }

        public int SortCode { get; private set; }

        public int IsEnabled { get; private set; }

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
            if (!(obj is GroupState))
            {
                return false;
            }
            var left = this;
            var right = (GroupState)obj;

            return left.Id == right.Id &&
                left.Name == right.Name &&
                left.OrganizationCode == right.OrganizationCode &&
                left.CategoryCode == right.CategoryCode &&
                left.SortCode == right.SortCode &&
                left.IsEnabled == right.IsEnabled;
        }

        public static bool operator ==(GroupState a, GroupState b)
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

        public static bool operator !=(GroupState a, GroupState b)
        {
            return !(a == b);
        }
    }
}

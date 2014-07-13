
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using System;
    using Util;

    public sealed class ResourceTypeState : IResourceType
    {
        public static readonly ResourceTypeState Empty = new ResourceTypeState
        {
            AppSystemID = Guid.Empty,
            AllowDelete = 0,
            AllowEdit = 0,
            Code = string.Empty,
            CreateOn = SystemTime.MinDate,
            Icon = string.Empty,
            Id = Guid.Empty,
            Name = string.Empty,
            SortCode = 0
        };

        private ResourceTypeState() { }

        public static ResourceTypeState Create(ResourceTypeBase resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException("resource");
            }
            return new ResourceTypeState
            {
                Id = resource.Id,
                AppSystemID = resource.AppSystemID,
                Name = resource.Name,
                Code = resource.Code,
                Icon = resource.Icon,
                SortCode = resource.SortCode,
                CreateOn = resource.CreateOn,
                AllowDelete = resource.AllowDelete,
                AllowEdit = resource.AllowEdit
            };
        }

        public Guid Id { get; private set; }

        public Guid AppSystemID { get; private set; }

        public string Name { get; private set; }

        public string Code { get; private set; }

        public string Icon { get; private set; }

        public int SortCode { get; private set; }

        public DateTime? CreateOn { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int AllowEdit { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int AllowDelete { get; private set; }

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
            if (!(obj is ResourceTypeState))
            {
                return false;
            }
            var left = this;
            var right = (ResourceTypeState)obj;

            return left.Id == right.Id &&
                left.AppSystemID == right.AppSystemID &&
                left.Name == right.Name &&
                left.Code == right.Code &&
                left.Icon == right.Icon &&
                left.SortCode == right.SortCode;
        }

        public static bool operator ==(ResourceTypeState a, ResourceTypeState b)
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

        public static bool operator !=(ResourceTypeState a, ResourceTypeState b)
        {
            return !(a == b);
        }
    }
}

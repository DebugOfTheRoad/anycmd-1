
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using System;
    using Util;

    public sealed class OrganizationState : IOrganization
    {
        public static readonly OrganizationState VirtualRoot = new OrganizationState
        {
            CategoryCode = string.Empty,
            Code = string.Empty,
            CreateOn = SystemTime.MinDate,
            Description = string.Empty,
            Id = Guid.Empty,
            IsEnabled = 1,
            ModifiedOn = null,
            Name = "虚拟根",
            ShortName = "根",
            ParentCode = null,
            SortCode = 0,
            ContractorID = null
        };

        public static readonly OrganizationState Empty = new OrganizationState
        {
            CategoryCode = string.Empty,
            Code = string.Empty,
            CreateOn = SystemTime.MinDate,
            Description = string.Empty,
            Id = Guid.NewGuid(),
            IsEnabled = 1,
            ModifiedOn = null,
            Name = "无",
            ShortName = "无",
            ParentCode = null,
            ContractorID = null,
            SortCode = 0
        };

        private OrganizationState() { }

        public static OrganizationState Create(IAppHost host, OrganizationBase organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException("organization");
            }
            return new OrganizationState
            {
                AppHost = host,
                CategoryCode = organization.CategoryCode,
                Code = organization.Code,
                CreateOn = organization.CreateOn,
                Description = organization.Description,
                Id = organization.Id,
                IsEnabled = organization.IsEnabled,
                ModifiedOn = organization.ModifiedOn,
                Name = organization.Name,
                ShortName = organization.ShortName,
                ParentCode = organization.ParentCode,
                SortCode = organization.SortCode,
                ContractorID = organization.ContractorID
            };
        }

        public IAppHost AppHost { get; private set; }

        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public string ShortName { get; private set; }

        public string ParentCode { get; private set; }

        public string CategoryCode { get; private set; }

        public Guid? ContractorID { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public DateTime? ModifiedOn { get; private set; }

        public string Description { get; private set; }

        public int IsEnabled { get; private set; }

        public int SortCode { get; private set; }

        /// <summary>
        /// 返回Empty，不会返回null。
        /// 虚拟根的父级是Empty。Empty没有父级
        /// </summary>
        public OrganizationState Parent
        {
            get
            {
                if (this.Equals(OrganizationState.Empty))
                {
                    throw new InvalidOperationException("不能访问Null组织结构的父级");
                }
                if (this.Equals(OrganizationState.VirtualRoot))
                {
                    return OrganizationState.Empty;
                }
                if (string.IsNullOrEmpty(this.ParentCode))
                {
                    return OrganizationState.VirtualRoot;
                }
                OrganizationState _parent;
                if (!AppHost.OrganizationSet.TryGetOrganization(this.ParentCode, out _parent))
                {
                    return OrganizationState.Empty;
                }

                return _parent;
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
            if (!(obj is OrganizationState))
            {
                return false;
            }
            var left = this;
            var right = (OrganizationState)obj;

            return left.Id == right.Id &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.ShortName == right.ShortName &&
                left.ParentCode == right.ParentCode &&
                left.CategoryCode == right.CategoryCode &&
                left.IsEnabled == right.IsEnabled &&
                left.ContractorID == right.ContractorID &&
                left.SortCode == right.SortCode &&
                left.Description == right.Description;
        }

        public static bool operator ==(OrganizationState a, OrganizationState b)
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

        public static bool operator !=(OrganizationState a, OrganizationState b)
        {
            return !(a == b);
        }
    }
}

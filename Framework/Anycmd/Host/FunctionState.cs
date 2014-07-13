
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using Exceptions;
    using System;
    using Util;

    public sealed class FunctionState : IFunction
    {
        private Guid _resourceTypeID;

        public static readonly FunctionState Empty = new FunctionState
        {
            Code = string.Empty,
            CreateOn = SystemTime.MinDate,
            Description = string.Empty,
            DeveloperID = Guid.Empty,
            Id = Guid.Empty,
            _resourceTypeID = Guid.Empty,
            IsEnabled = 0,
            IsManaged = false,
            SortCode = 0
        };

        private FunctionState() { }

        public static FunctionState Create(AppHost host, FunctionBase function)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            if (function.ResourceTypeID == Guid.Empty)
            {
                throw new CoreException("必须指定资源");
            }
            ResourceTypeState resource;
            if (!host.ResourceSet.TryGetResource(function.ResourceTypeID, out resource))
            {
                throw new ValidationException("非法的资源标识" + function.ResourceTypeID);
            }
            return new FunctionState
            {
                AppHost = host,
                Id = function.Id,
                ResourceTypeID = function.ResourceTypeID,
                Code = function.Code,
                IsManaged = function.IsManaged,
                IsEnabled = function.IsEnabled,
                DeveloperID = function.DeveloperID,
                SortCode = function.SortCode,
                Description = function.Description,
                CreateOn = function.CreateOn
            };
        }

        public AppHost AppHost { get; private set; }

        public Guid Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Guid ResourceTypeID
        {
            get { return _resourceTypeID; }
            private set
            {
                ResourceTypeState resource;
                if (!AppHost.ResourceSet.TryGetResource(value, out resource))
                {
                    throw new ValidationException("意外的功能资源标识" + value);
                }
                _resourceTypeID = value;
            }
        }

        public string Code { get; private set; }

        public bool IsManaged { get; private set; }

        public int IsEnabled { get; private set; }

        public Guid DeveloperID { get; set; }

        public int SortCode { get; private set; }

        public string Description { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public AppSystemState AppSystem
        {
            get
            {
                if (this == Empty)
                {
                    return AppSystemState.Empty;
                }
                AppSystemState appSystem;
                if (!AppHost.AppSystemSet.TryGetAppSystem(this.Resource.AppSystemID, out appSystem))
                {
                    throw new CoreException("意外的应用系统标识");
                }
                return appSystem;
            }
        }

        public ResourceTypeState Resource
        {
            get
            {
                if (this == Empty)
                {
                    return ResourceTypeState.Empty;
                }
                ResourceTypeState resource;
                if (!AppHost.ResourceSet.TryGetResource(this.ResourceTypeID, out resource))
                {
                    throw new CoreException("意外的资源标识");
                }
                return resource;
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
            if (!(obj is FunctionState))
            {
                return false;
            }
            var left = this;
            var right = (FunctionState)obj;

            return left.Id == right.Id &&
                left.ResourceTypeID == right.ResourceTypeID &&
                left.Code == right.Code &&
                left.IsManaged == right.IsManaged &&
                left.IsEnabled == right.IsEnabled &&
                left.DeveloperID == right.DeveloperID &&
                left.SortCode == right.SortCode &&
                left.Description == right.Description;
        }

        public static bool operator ==(FunctionState a, FunctionState b)
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

        public static bool operator !=(FunctionState a, FunctionState b)
        {
            return !(a == b);
        }
    }
}

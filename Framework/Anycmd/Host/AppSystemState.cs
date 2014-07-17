
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using Exceptions;
    using System;
    using Util;

    public sealed class AppSystemState : IAppSystem
    {
        public static readonly AppSystemState Empty = new AppSystemState
        {
            Code = string.Empty,
            CreateOn = SystemTime.MinDate,
            Icon = string.Empty,
            Id = Guid.Empty,
            IsEnabled = 0,
            Name = string.Empty,
            PrincipalID = Guid.Empty,
            SortCode = 0,
            SSOAuthAddress = string.Empty
        };

        private AppSystemState() { }

        public static AppSystemState Create(AppHost host, AppSystemBase appSystem)
        {
            if (appSystem == null)
            {
                throw new ArgumentNullException("appSystem");
            }
            AccountState principal;
            if (!host.SysUsers.TryGetDevAccount(appSystem.PrincipalID, out principal))
            {
                throw new CoreException("意外的应用系统负责人标识" + appSystem.PrincipalID);
            }
            return new AppSystemState
            {
                Id = appSystem.Id,
                Code = appSystem.Code,
                Name = appSystem.Name,
                SortCode = appSystem.SortCode,
                PrincipalID = appSystem.PrincipalID,
                IsEnabled = appSystem.IsEnabled,
                SSOAuthAddress = appSystem.SSOAuthAddress,
                Icon = appSystem.Icon,
                CreateOn = appSystem.CreateOn,
            };
        }

        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public int SortCode { get; set; }

        public Guid PrincipalID { get; set; }

        public int IsEnabled { get; private set; }

        public string SSOAuthAddress { get; private set; }

        public string Icon { get; set; }

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
            if (!(obj is AppSystemState))
            {
                return false;
            }
            var left = this;
            var right = (AppSystemState)obj;

            return
                left.Id == right.Id &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.SortCode == right.SortCode &&
                left.IsEnabled == right.IsEnabled &&
                left.SSOAuthAddress == right.SSOAuthAddress &&
                left.Icon == right.Icon;
        }

        public static bool operator ==(AppSystemState a, AppSystemState b)
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

        public static bool operator !=(AppSystemState a, AppSystemState b)
        {
            return !(a == b);
        }
    }
}

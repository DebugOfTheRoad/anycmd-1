
namespace Anycmd.Host
{
    using Anycmd.AC.Identity;
    using System;
    using Util;

    public sealed class AccountState : IAccount
    {
        /// <summary>
        /// 空账户
        /// </summary>
        public static readonly AccountState Empty = new AccountState
        {
            NumberID = int.MinValue,
            CreateOn = SystemTime.MinDate,
            Id = Guid.Empty,
            LoginName = string.Empty,
            Theme = string.Empty,
            Wallpaper = string.Empty,
            BackColor = string.Empty,
            Code = string.Empty,
            Email = string.Empty,
            Mobile = string.Empty,
            Name = string.Empty,
            QQ = string.Empty,
            Telephone = string.Empty
        };

        private AccountState() { }

        public static AccountState Create(AccountBase account)
        {
            if (account == null)
            {
                return Empty;
            }
            return new AccountState()
            {
                NumberID = account.NumberID,
                Id = account.Id,
                LoginName = account.LoginName,
                BackColor = account.BackColor,
                CreateOn = account.CreateOn,
                Theme = account.Theme,
                Wallpaper = account.Wallpaper,
                Code = account.Code,
                Email = account.Email,
                Mobile = account.Mobile,
                Name = account.Name,
                QQ = account.QQ,
                Telephone = account.Telephone
            };
        }

        public int NumberID { get; private set; }

        public string LoginName { get; private set; }

        public string Theme { get; private set; }

        public string Wallpaper { get; private set; }

        public string BackColor { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public Guid Id { get; set; }

        public string Name { get; private set; }

        public string Code { get; private set; }

        public string Email { get; private set; }

        public string QQ { get; private set; }

        public string Mobile { get; private set; }

        public string Telephone { get; private set; }
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
            if (!(obj is AccountState))
            {
                return false;
            }
            var left = this;
            var right = (AccountState)obj;

            return left.Id == right.Id &&
                left.LoginName == right.LoginName &&
                left.NumberID == right.NumberID &&
                left.Name == right.Name &&
                left.Code == right.Code &&
                left.Email == right.Email &&
                left.QQ == right.QQ &&
                left.Mobile == right.Mobile &&
                left.Telephone == right.Telephone;
        }

        public static bool operator ==(AccountState a, AccountState b)
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

        public static bool operator !=(AccountState a, AccountState b)
        {
            return !(a == b);
        }
    }
}

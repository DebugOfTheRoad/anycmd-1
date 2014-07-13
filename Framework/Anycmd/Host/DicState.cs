
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using System;
    using Util;

    public sealed class DicState : IDic
    {
        public static readonly DicState Empty = new DicState
        {
            Code = string.Empty,
            CreateOn = SystemTime.MinDate,
            Id = Guid.Empty,
            IsEnabled = 0,
            Name = string.Empty,
            SortCode = 0
        };

        private DicState() { }

        public static DicState Create(DicBase dic)
        {
            if (dic == null)
            {
                throw new ArgumentNullException("dic");
            }
            return new DicState
            {
                Id = dic.Id,
                Code = dic.Code,
                Name = dic.Name,
                IsEnabled = dic.IsEnabled,
                CreateOn = dic.CreateOn,
                SortCode = dic.SortCode
            };
        }

        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public int IsEnabled { get; private set; }

        public int SortCode { get; private set; }

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
            if (!(obj is DicState))
            {
                return false;
            }
            var left = this;
            var right = (DicState)obj;

            return left.Id == right.Id &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.IsEnabled == right.IsEnabled &&
                left.SortCode == right.SortCode;
        }

        public static bool operator ==(DicState a, DicState b)
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

        public static bool operator !=(DicState a, DicState b)
        {
            return !(a == b);
        }
    }
}

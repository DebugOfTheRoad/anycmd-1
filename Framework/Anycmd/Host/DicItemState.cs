
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using Exceptions;
    using System;
    using Util;

    public sealed class DicItemState : IDicItem
    {
        public static readonly DicItemState Empty = new DicItemState
        {
            CreateOn = SystemTime.MinDate,
            IsEnabled = 0,
            SortCode = 0,
            Code = string.Empty,
            DicID = Guid.Empty,
            Id = Guid.Empty,
            Name = string.Empty
        };

        private DicItemState() { }

        public static DicItemState Create(AppHost host, DicItemBase dicItem)
        {
            if (dicItem == null)
            {
                throw new ArgumentNullException("dicItem");
            }
            if (!host.DicSet.ContainsDic(dicItem.DicID))
            {
                throw new CoreException("意外的字典" + dicItem.DicID);
            }
            return new DicItemState
            {
                Id = dicItem.Id,
                Code = dicItem.Code,
                Name = dicItem.Name,
                DicID = dicItem.DicID,
                SortCode = dicItem.SortCode,
                IsEnabled = dicItem.IsEnabled,
                CreateOn = dicItem.CreateOn
            };
        }

        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public Guid DicID { get; private set; }

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
            if (!(obj is DicItemState))
            {
                return false;
            }
            var left = this;
            var right = (DicItemState)obj;

            return left.Id == right.Id &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.DicID == right.DicID &&
                left.SortCode == right.SortCode &&
                left.IsEnabled == right.IsEnabled;
        }

        public static bool operator ==(DicItemState a, DicItemState b)
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

        public static bool operator !=(DicItemState a, DicItemState b)
        {
            return !(a == b);
        }
    }
}

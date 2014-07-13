
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using System;

    public sealed class InfoDicItemState : IInfoDicItem
    {
        private InfoDicItemState() { }

        public static InfoDicItemState Create(IInfoDicItem infoDicItem)
        {
            if (infoDicItem == null)
            {
                throw new ArgumentNullException("infoDicItem");
            }
            return new InfoDicItemState
            {
                Code = infoDicItem.Code,
                CreateOn = infoDicItem.CreateOn,
                Description = infoDicItem.Description,
                Id = infoDicItem.Id,
                InfoDicID = infoDicItem.InfoDicID,
                IsEnabled = infoDicItem.IsEnabled,
                Level = infoDicItem.Level,
                ModifiedOn = infoDicItem.ModifiedOn,
                Name = infoDicItem.Name,
                SortCode = infoDicItem.SortCode
            };
        }

        public Guid Id { get; private set; }

        public string Level { get; private set; }

        public string Code { get; private set; }

        public int IsEnabled { get; private set; }

        public Guid InfoDicID { get; private set; }

        public string Name { get; private set; }

        public int SortCode { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public DateTime? ModifiedOn { get; private set; }

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
            if (!(obj is InfoDicItemState))
            {
                return false;
            }
            var left = this;
            var right = (InfoDicItemState)obj;

            return
                left.Id == right.Id &&
                left.Level == right.Level &&
                left.Code == right.Code &&
                left.IsEnabled == right.IsEnabled &&
                left.InfoDicID == right.InfoDicID &&
                left.SortCode == right.SortCode &&
                left.Name == right.Name;
        }

        public static bool operator ==(InfoDicItemState a, InfoDicItemState b)
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

        public static bool operator !=(InfoDicItemState a, InfoDicItemState b)
        {
            return !(a == b);
        }
    }
}

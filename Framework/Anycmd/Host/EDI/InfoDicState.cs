
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using System;

    public sealed class InfoDicState : IInfoDic
    {
        private InfoDicState() { }

        public static InfoDicState Create(IInfoDic infoDic)
        {
            if (infoDic == null)
            {
                throw new ArgumentNullException("infoDic");
            }
            return new InfoDicState
            {
                Code = infoDic.Code,
                CreateOn = infoDic.CreateOn,
                Id = infoDic.Id,
                IsEnabled = infoDic.IsEnabled,
                Name = infoDic.Name,
                SortCode = infoDic.SortCode
            };
        }

        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public int IsEnabled { get; private set; }

        public int SortCode { get; private set; }

        public string Name { get; private set; }

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
            if (!(obj is InfoDicState))
            {
                return false;
            }
            var left = this;
            var right = (InfoDicState)obj;

            return
                left.Id == right.Id &&
                left.Code == right.Code &&
                left.IsEnabled == right.IsEnabled &&
                left.SortCode == right.SortCode &&
                left.Name == right.Name;
        }

        public static bool operator ==(InfoDicState a, InfoDicState b)
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

        public static bool operator !=(InfoDicState a, InfoDicState b)
        {
            return !(a == b);
        }
    }
}

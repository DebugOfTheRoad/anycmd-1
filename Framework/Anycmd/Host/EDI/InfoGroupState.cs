using Anycmd.EDI;
using System;

namespace Anycmd.Host.EDI
{
    public sealed class InfoGroupState : IInfoGroup
    {
        private InfoGroupState() { }

        public static InfoGroupState Create(IInfoGroup infoGroup)
        {
            if (infoGroup == null)
            {
                throw new ArgumentNullException("infoGroup");
            }
            return new InfoGroupState
            {
                Code = infoGroup.Code,
                Description = infoGroup.Description,
                Id = infoGroup.Id,
                Name = infoGroup.Name,
                OntologyID = infoGroup.OntologyID,
                SortCode = infoGroup.SortCode
            };
        }

        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public Guid OntologyID { get; private set; }

        public int SortCode { get; private set; }

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
            if (!(obj is InfoGroupState))
            {
                return false;
            }
            var left = this;
            var right = (InfoGroupState)obj;

            return
                left.Id == right.Id &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.OntologyID == right.OntologyID &&
                left.SortCode == right.SortCode &&
                left.Description == right.Description;
        }

        public static bool operator ==(InfoGroupState a, InfoGroupState b)
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

        public static bool operator !=(InfoGroupState a, InfoGroupState b)
        {
            return !(a == b);
        }
    }
}

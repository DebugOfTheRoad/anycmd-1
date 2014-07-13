using System;

namespace Anycmd.Host
{
    using Anycmd.AC;
    using Exceptions;
    using Util;

    public sealed class PrivilegeBigramState
    {
        private PrivilegeBigramState() { }

        public static PrivilegeBigramState Create(PrivilegeBigramBase privilegeBigram)
        {
            if (privilegeBigram == null)
            {
                throw new ArgumentNullException("privilegeBigram");
            }
            if (string.IsNullOrEmpty(privilegeBigram.SubjectType))
            {
                throw new CoreException("必须指定主授权授权类型");
            }
            if (string.IsNullOrEmpty(privilegeBigram.ObjectType))
            {
                throw new CoreException("必须指定授权授权类型");
            }
            ACSubjectType subjectType;
            ACObjectType acObjectType;
            if (!privilegeBigram.SubjectType.TryParse(out subjectType))
            {
                throw new CoreException("非法的主授权类型" + privilegeBigram.SubjectType);
            }
            if (!privilegeBigram.ObjectType.TryParse(out acObjectType))
            {
                throw new CoreException("非法的从授权类型" + privilegeBigram.ObjectType);
            }
            return new PrivilegeBigramState
            {
                Id = privilegeBigram.Id,
                SubjectType = subjectType,
                SubjectInstanceID = privilegeBigram.SubjectInstanceID,
                ObjectType = acObjectType,
                ObjectInstanceID = privilegeBigram.ObjectInstanceID,
                PrivilegeConstraint = privilegeBigram.PrivilegeConstraint,
                CreateOn = privilegeBigram.CreateOn,
                CreateBy = privilegeBigram.CreateBy,
                CreateUserID = privilegeBigram.CreateUserID,
                PrivilegeOrientation = privilegeBigram.PrivilegeOrientation
            };
        }

        public Guid Id { get; private set; }

        public ACSubjectType SubjectType { get; private set; }

        public Guid SubjectInstanceID { get; private set; }

        public ACObjectType ObjectType { get; private set; }

        public Guid ObjectInstanceID { get; private set; }

        public string PrivilegeConstraint { get; private set; }

        public int PrivilegeOrientation { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public string CreateBy { get; private set; }

        public Guid? CreateUserID { get; private set; }

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
            if (!(obj is PrivilegeBigramState))
            {
                return false;
            }
            var left = this;
            var right = (PrivilegeBigramState)obj;

            return left.Id == right.Id &&
                left.SubjectType == right.SubjectType &&
                left.SubjectInstanceID == right.SubjectInstanceID &&
                left.ObjectType == right.ObjectType &&
                left.ObjectInstanceID == right.ObjectInstanceID &&
                left.PrivilegeConstraint == right.PrivilegeConstraint &&
                left.PrivilegeOrientation == right.PrivilegeOrientation;
        }

        public static bool operator ==(PrivilegeBigramState a, PrivilegeBigramState b)
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

        public static bool operator !=(PrivilegeBigramState a, PrivilegeBigramState b)
        {
            return !(a == b);
        }
    }
}

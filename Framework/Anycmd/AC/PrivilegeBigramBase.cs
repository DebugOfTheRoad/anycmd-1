
namespace Anycmd.AC
{
    using Exceptions;
    using Model;
    using System;

    public abstract class PrivilegeBigramBase : EntityBase, IPrivilegeBigram
    {
        private string subjectType;
        private Guid subjectInstanceID;
        private string objectType;
        private Guid objectInstanceID;

        public virtual string SubjectType {
            get { return subjectType; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new CoreException("必须指定主授权类型");
                }
                if (value != subjectType && subjectType != null)
                {
                    throw new CoreException("主授权类型不能更改");
                }
                subjectType = value;
            }
        }

        public virtual Guid SubjectInstanceID
        {
            get { return subjectInstanceID; }
            set
            {
                if (value == Guid.Empty)
                {
                    throw new CoreException("必须指定主授权类型");
                }
                if (value != subjectInstanceID && subjectInstanceID != Guid.Empty)
                {
                    throw new CoreException("主授权类型不能更改");
                }
                subjectInstanceID = value;
            }
        }

        public virtual string ObjectType
        {
            get { return objectType; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new CoreException("必须指定从授权类型");
                }
                if (value != objectType && objectType != null)
                {
                    throw new CoreException("从授权类型不能更改");
                }
                objectType = value;
            }
        }

        public virtual Guid ObjectInstanceID
        {
            get { return objectInstanceID; }
            set
            {
                if (value == Guid.Empty)
                {
                    throw new CoreException("必须指定从授权类型");
                }
                if (value != objectInstanceID && objectInstanceID != Guid.Empty)
                {
                    throw new CoreException("从授权类型不能更改");
                }
                objectInstanceID = value;
            }
        }

        public virtual int PrivilegeOrientation { get; set; }

        public virtual string PrivilegeConstraint { get; set; }
    }
}

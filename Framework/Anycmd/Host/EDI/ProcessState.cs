using System;

namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;
    using Util;

    public sealed class ProcessState : IProcess
    {
        private string _type;

        private ProcessState() { }

        public static ProcessState Create(IProcess process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }
            return new ProcessState
            {
                CreateOn = process.CreateOn,
                Description = process.Description,
                Id = process.Id,
                IsEnabled = process.IsEnabled,
                Name = process.Name,
                NetPort = process.NetPort,
                OntologyID = process.OntologyID,
                OrganizationCode = process.OrganizationCode,
                Type = process.Type
            };
        }

        public Guid Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type
        {
            get { return _type; }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("进程类型不能为空");
                }
                ProcessType processType;
                if (!value.TryParse(out processType))
                {
                    throw new CoreException("非法的进程类型");
                }
                _type = value;
            }
        }

        public string Name { get; private set; }

        public int NetPort { get; private set; }

        public int IsEnabled { get; private set; }

        public Guid OntologyID { get; private set; }

        public string OrganizationCode { get; private set; }

        public string Description { get; private set; }

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
            if (!(obj is ProcessState))
            {
                return false;
            }
            var left = this;
            var right = (ProcessState)obj;

            return
                left.Id == right.Id &&
                left.Type == right.Type &&
                left.Name == right.Name &&
                left.NetPort == right.NetPort &&
                left.IsEnabled == right.IsEnabled &&
                left.OntologyID == right.OntologyID &&
                left.OrganizationCode == right.OrganizationCode;
        }

        public static bool operator ==(ProcessState a, ProcessState b)
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

        public static bool operator !=(ProcessState a, ProcessState b)
        {
            return !(a == b);
        }
    }
}

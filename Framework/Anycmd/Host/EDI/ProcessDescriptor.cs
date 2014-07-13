
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Anycmd.Host;
    using Exceptions;
    using System;
    using System.Net;
    using Util;

    /// <summary>
    /// 进程描述对象。进程描述对象上可以读取进程配置信息。描述对象往往长久贮存在内存中。
    /// </summary>
    public sealed class ProcessDescriptor
    {
        private OntologyDescriptor _ontology;
        ProcessType _type;
        private string _hostName;
        private string _webApiBaseAddress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        public ProcessDescriptor(ProcessState process)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }
            this.Process = process;
            if (!process.Type.TryParse(out _type))
            {
                throw new CoreException("意外的进程类型");
            }
            if (!string.IsNullOrEmpty(process.OrganizationCode))
            {
                OrganizationState org;
                if (!NodeHost.Instance.AppHost.OrganizationSet.TryGetOrganization(process.OrganizationCode, out org))
                {
                    throw new CoreException("意外的组织结构码" + process.OrganizationCode);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ProcessState Process { get; private set; }

        /// <summary>
        /// 进程类型
        /// </summary>
        public ProcessType ProcessType
        {
            get { return _type; }
            private set { _type = value; }
        }

        /// <summary>
        /// 标题。由本节点名称+本进程名称+本进程绑定的本体的名称拼接而来
        /// </summary>
        public string Title
        {
            get
            {
                return NodeHost.Instance.Nodes.ThisNode.Node.Name + this.Process.Name + " - " + this.Ontology.Ontology.Name;
            }
        }

        /// <summary>
        /// 本体。进程必须关联一个本体。一个进程只能处理一个本体的事务，不能既处理教师又处理学生。
        /// </summary>
        public OntologyDescriptor Ontology
        {
            get
            {
                if (_ontology == null)
                {
                    if (!NodeHost.Instance.Ontologies.TryGetOntology(this.Process.OntologyID, out _ontology))
                    {
                        throw new CoreException("非法本体标识" + this.Process.OntologyID);
                    }
                }
                return _ontology;
            }
        }

        /// <summary>
        /// 进程标识。使用Url来标识一个进程。
        /// </summary>
        public string WebApiBaseAddress
        {
            get
            {
                if (_webApiBaseAddress == null)
                {
                    _webApiBaseAddress = "http://" + HostName + ":" + this.Process.NetPort.ToString() + "/";
                }
                return _webApiBaseAddress;
            }
        }

        /// <summary>
        /// 进程所在的主机名。
        /// </summary>
        private string HostName
        {
            get
            {
                if (_hostName == null)
                {
                    _hostName = this.Ontology.EntityProvider.GetEntityDataSource(this.Ontology);
                }
                return _hostName;
            }
        }

        /// <summary>
        /// 注意：每次访问都会引发网络请求。以Http Get请求GetWebApiBaseAddress+IsAlive地址。
        /// </summary>
        /// <returns></returns>
        public bool IsRuning()
        {
            bool isOnline = false;
            var r = HttpWebRequest.Create(this.WebApiBaseAddress + "IsAlive") as HttpWebRequest;
            r.Method = "GET";
            try
            {
                var s = r.GetResponse() as HttpWebResponse;
                if (s.StatusCode == HttpStatusCode.OK)
                {
                    isOnline = true;
                }
            }
            catch
            {
                isOnline = false;
            }
            return isOnline;
        }

        public override int GetHashCode()
        {
            return this.Process.Id.GetHashCode();
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
            if (!(obj is ProcessDescriptor))
            {
                return false;
            }
            var left = this;
            var right = (ProcessDescriptor)obj;

            return left.Process == right.Process;
        }

        public static bool operator ==(ProcessDescriptor a, ProcessDescriptor b)
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

        public static bool operator !=(ProcessDescriptor a, ProcessDescriptor b)
        {
            return !(a == b);
        }
    }
}

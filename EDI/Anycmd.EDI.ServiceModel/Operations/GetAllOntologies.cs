﻿
namespace Anycmd.EDI.ServiceModel.Operations {
    using DataContracts;
    using ServiceStack;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Types;

    /// <summary>
    /// 数据传输模型：请求获取全部本体的请求模型
    /// </summary>
    [Route("/Ontology/GetAll")]
    [DataContract]
    [Api("全部本体。")]
    public sealed class GetAllOntologies : IReturn<GetAllOntologiesResponse>, IDto {
    }

    /// <summary>
    /// 数据传输模型：请求获取全部本体的响应模型
    /// </summary>
    [DataContract]
    public sealed class GetAllOntologiesResponse {
        private static readonly string hostName = System.Net.Dns.GetHostName();
        private string serverID;
        private long serverTicks = DateTime.UtcNow.Ticks;

        public GetAllOntologiesResponse() {
            Ontologies = new List<OntologyData>();
            this.Description = "本体列表";
        }

        /// <summary>
        /// 命令状态码。
        /// </summary>
        [DataMember]
        public int Status { get; set; }

        /// <summary>
        /// 原因短语。
        /// </summary>
        [DataMember]
        public string ReasonPhrase { get; set; }

        /// <summary>
        /// 命令状态码描述。
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// 服务器响应时间戳
        /// </summary>
        [DataMember]
        public long ServerTicks {
            get { return serverTicks; }
            set { serverTicks = value; }
        }

        /// <summary>
        /// 服务器标识
        /// </summary>
        [DataMember]
        public string ServerID {
            get {
                if (serverID == null) {
                    return hostName;
                }
                return serverID;
            }
            set {
                serverID = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public List<OntologyData> Ontologies { get; set; }
    }
}

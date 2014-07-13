﻿
namespace Anycmd.EDI.ServiceModel.Types {
    using DataContracts;
    using System.Runtime.Serialization;

    /// <summary>
    /// 数据传输模型：组织结构
    /// </summary>
    [DataContract]
    public sealed class OrganizationData : IDto {
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Order = 10)]
        public string ParentCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Order = 20)]
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Order = 30)]
        public string Name { get; set; }
    }
}


namespace Anycmd.EDI {
    using Exceptions;
    using Model;
    using System;
    using Util;

    /// <summary>
    /// 进程
    /// </summary>
    public abstract class ProcessBase : EntityBase, IProcess {
        private string _type;
        private Guid _ontologyID;

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Type {
            get { return _type; }
            set {
                if (value != _type) {
                    if (_type != null) {
                        throw new CoreException("不能更改进程类型");
                    }
                    else if (value == null) {
                        throw new CoreException("必须指定进程类型");
                    }
                    ProcessType processType;
                    if (!value.TryParse(out processType)) {
                        throw new CoreException("非法的进程类型");
                    }
                    _type = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NetPort { get; set; }
        /// <summary>
        /// 有效标记
        /// </summary>
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID {
            get { return _ontologyID; }
            set {
                if (value != _ontologyID) {
                    if (_ontologyID != Guid.Empty) {
                        throw new CoreException("不能更改关联本体");
                    }
                    _ontologyID = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OrganizationCode { get; set; }
    }
}


namespace Anycmd.EDI {
    using Exceptions;
    using Model;
    using System;

    public abstract class OntologyOrganizationBase : EntityBase, IOntologyOrganization {
        private Guid _orgnizationID;
        private Guid _ontologyID;

        protected OntologyOrganizationBase() { }

        /// <summary>
        /// 组织结构标识
        /// </summary>
        public Guid OrganizationID {
            get { return _orgnizationID; }
            set {
                if (value != _orgnizationID) {
                    if (_orgnizationID != Guid.Empty) {
                        throw new CoreException("不能更改所属组织结构");
                    }
                    _orgnizationID = value;
                }
            }
        }

        /// <summary>
        /// 本体标识
        /// </summary>
        public Guid OntologyID {
            get { return _ontologyID; }
            set {
                if (value != _ontologyID) {
                    if (_ontologyID != Guid.Empty) {
                        throw new CoreException("不能更改所属本体");
                    }
                    _ontologyID = value;
                }
            }
        }

        public string Actions { get; set; }
    }
}


namespace Anycmd.EDI {
    using Exceptions;
    using Model;
    using System;

    public abstract class NodeElementCareBase : EntityBase, INodeElementCare {
        #region Private Fields
        private Guid _nodeID;
        private Guid _elementID;
        private bool _isInfoID;
        #endregion

        #region Public Properties
        /// <summary>
        /// 节点ID
        /// </summary>
        public Guid NodeID {
            get { return _nodeID; }
            set {
                if (value != _nodeID) {
                    if (_nodeID != Guid.Empty) {
                        throw new CoreException("关联节点不能更改");
                    }
                    _nodeID = value;
                }
            }
        }
        /// <summary>
        /// 本体元素主键
        /// </summary>
        public Guid ElementID {
            get { return _elementID; }
            set {
                if (value != _elementID) {
                    if (_elementID != Guid.Empty) {
                        throw new CoreException("关联本体元素不能更改");
                    }
                    _elementID = value;
                }
            }
        }
        /// <summary>
        /// 是否是信息标识本体元素
        /// </summary>
        public bool IsInfoIDItem {
            get { return _isInfoID; }
            set {
                if (value != _isInfoID) {
                    _isInfoID = value;
                }
            }
        }
        #endregion
    }
}

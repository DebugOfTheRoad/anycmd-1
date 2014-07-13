
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class NodeTopic : EntityBase, IAggregateRoot, INodeTopic
    {
        private Guid nodeID;
        private Guid eventSubjectID;

        public NodeTopic() { }

        /// <summary>
        /// 
        /// </summary>
        public Guid NodeID
        {
            get { return nodeID; }
            set
            {
                if (value != nodeID)
                {
                    if (nodeID != Guid.Empty)
                    {
                        throw new CoreException("不能更改关联节点");
                    }
                    nodeID = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid TopicID
        {
            get { return eventSubjectID; }
            set
            {
                if (value != eventSubjectID)
                {
                    if (eventSubjectID != Guid.Empty)
                    {
                        throw new CoreException("不能更改关联事件主题");
                    }
                    eventSubjectID = value;
                }
            }
        }
    }
}

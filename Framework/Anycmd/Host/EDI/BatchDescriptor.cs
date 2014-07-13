
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;
    using System;
    using Util;

    /// <summary>
    /// “批”描述对象。
    /// </summary>
    public class BatchDescriptor
    {
        private readonly IBatch batch;
        private BatchType _batchType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="batch"></param>
        public BatchDescriptor(IBatch batch)
        {
            if (batch == null)
            {
                throw new ArgumentNullException("batch");
            }
            this.batch = batch;
            OntologyDescriptor _ontology;
            if (!NodeHost.Instance.Ontologies.TryGetOntology(batch.OntologyID, out _ontology))
            {
                throw new CoreException("意外的本体标识" + batch.OntologyID);
            }
            this.Ontology = _ontology;
            NodeDescriptor _node;
            if (!NodeHost.Instance.Nodes.TryGetNodeByID(batch.NodeID.ToString(), out _node))
            {
                throw new CoreException("意外的节点标识" + batch.NodeID);
            }
            this.Node = _node;
            if (!batch.Type.TryParse(out _batchType))
            {
                throw new CoreException("意外的批类型" + batch.Type);
            }
        }

        /// <summary>
        /// 批类型
        /// </summary>
        public BatchType Type
        {
            get { return _batchType; }
        }

        /// <summary>
        /// 本体
        /// </summary>
        public OntologyDescriptor Ontology
        {
            get;
            private set;
        }

        /// <summary>
        /// 可能为null
        /// </summary>
        public NodeDescriptor Node
        {
            get;
            private set;
        }
    }
}

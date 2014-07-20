
namespace Anycmd.EDI.ViewModels.NodeViewModels
{
    using Host.EDI;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public partial class NodeTr
    {
        private bool _isSelfDetected = false;
        private bool _isSelf = false;
        private bool _isCenterDetected = false;
        private bool _isCenter = false;
        private readonly IAppHost host;

        private NodeTr(IAppHost host)
        {
            this.host = host;
        }

        public static NodeTr Create(NodeDescriptor node)
        {
            return new NodeTr(node.Host)
            {
                AnycmdApiAddress = node.Node.AnycmdApiAddress,
                AnycmdWSAddress = node.Node.AnycmdWSAddress,
                BeatPeriod = node.Node.BeatPeriod,
                Code = node.Node.Code,
                CreateOn = node.Node.CreateOn,
                Email = node.Node.Email,
                Icon = node.Node.Icon,
                Id = node.Node.Id,
                IsDistributeEnabled = node.Node.IsDistributeEnabled,
                IsEnabled = node.Node.IsEnabled,
                IsExecuteEnabled = node.Node.IsExecuteEnabled,
                IsProduceEnabled = node.Node.IsProduceEnabled,
                IsReceiveEnabled = node.Node.IsReceiveEnabled,
                Mobile = node.Node.Mobile,
                Name = node.Node.Name,
                Organization = node.Node.Organization,
                PublicKey = node.Node.PublicKey,
                QQ = node.Node.QQ,
                SortCode = node.Node.SortCode,
                Steward = node.Node.Steward,
                Telephone = node.Node.Telephone,
                TransferID = node.Node.TransferID
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ���뵥λ����
        /// </summary>
        public string Organization { get; set; }
        /// <summary>
        /// רԱ
        /// </summary>
        public string Steward { get; set; }
        /// <summary>
        /// �̶��绰
        /// </summary>
        public string Telephone { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// �ֻ�����
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsExecuteEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsProduceEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsReceiveEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDistributeEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid TransferID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AnycmdApiAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AnycmdWSAddress { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? BeatPeriod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSelf
        {
            get
            {
                if (!_isSelfDetected)
                {
                    _isSelfDetected = true;
                    _isSelf = this.Id == host.Nodes.ThisNode.Node.Id;
                }
                return _isSelf;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCenter
        {
            get
            {
                if (!_isCenterDetected)
                {
                    _isCenterDetected = true;
                    _isCenter = this.Id == host.Nodes.CenterNode.Node.Id;
                }
                return _isCenter;
            }
        }
    }
}

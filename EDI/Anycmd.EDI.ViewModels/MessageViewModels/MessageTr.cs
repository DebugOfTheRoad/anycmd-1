
namespace Anycmd.EDI.MessageViewModels
{
    using Exceptions;
    using Host;
    using Host.EDI;
    using Host.EDI.Handlers;
    using Host.EDI.Hecp;
    using Host.EDI.Info;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Util;

    /// <summary>
    /// 实现命令视图模型的抽象泛型基类
    /// </summary>
    /// <typeparam name="TCommand">命令类型参数</typeparam>
    public class MessageTr : IMessageView
    {
        private MessageEntity command;
        private string clientName = null;
        private string ontologyName = null;
        OntologyDescriptor ontology;
        private string actionName = null;
        private string organizationName = null;
        private IList<InfoItem> _infoValueItems;
        private static readonly List<InfoItem> EmptyInfoValueItems = new List<InfoItem>();
        private string _commandInfo = null;
        private bool _isSelfDetected = false;
        private bool _isSelf = false;
        private bool _isCenterDetected = false;
        private bool _isCenter = false;
        private readonly IAppHost host;

        protected internal MessageTr(IAppHost host)
        {
            this.host = host;
        }

        /// <summary>
        /// 板上组装。提供给子类实现的模板方法。
        /// </summary>
        /// <param name="command"></param>
        protected virtual void PopulateCore(MessageEntity command)
        {

        }

        /// <summary>
        /// 工厂方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static MessageTr Create(IAppHost host, MessageEntity command)
        {
            var t = new MessageTr(host);
            t.Populate(command);

            return t;
        }

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
                    _isSelf = host.Config.ThisNodeID.Equals(this.ClientID, StringComparison.OrdinalIgnoreCase);
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
                    _isCenter = host.Config.CenterNodeID.Equals(this.ClientID, StringComparison.OrdinalIgnoreCase);
                }
                return _isCenter;
            }
        }

        public Guid Id { get; private set; }
        public string MessageID { get; private set; }
        public string MessageType { get; private set; }
        public string ClientType { get; private set; }
        public string ClientID { get; private set; }
        public string Verb { get; private set; }
        public string InfoFormat { get; private set; }
        public string InfoValue { get; private set; }
        public string Ontology { get; private set; }
        public string OrganizationCode { get; private set; }
        public string InfoID { get; private set; }
        public string LocalEntityID { get; private set; }
        public DateTime ReceivedOn { get; set; }
        public DateTime TimeStamp { get; private set; }
        public string UserName { get; private set; }
        public int StateCode { get; private set; }
        public string ReasonPhrase { get; private set; }
        public string Description { get; private set; }

        public string EventSubjectCode { get; set; }
        public string EventSourceType { get; set; }

        /// <summary>
        /// 账户标识
        /// </summary>
        public string Principal { get; set; }
        public DateTime CreateOn { get; private set; }

        public string OrganizationName
        {
            get
            {
                if (organizationName == null)
                {
                    OrganizationState org;
                    if (host.OrganizationSet.TryGetOrganization(this.OrganizationCode, out org))
                    {
                        organizationName = org.Name;
                    }
                    else
                    {
                        organizationName = string.Empty;
                    }
                    if (organizationName == null)
                    {
                        organizationName = string.Empty;
                    }
                }
                return organizationName;
            }
        }

        #region ClientName
        public string ClientName
        {
            get
            {
                if (clientName == null)
                {
                    NodeDescriptor node;
                    if (host.Nodes.TryGetNodeByID(this.ClientID, out node))
                    {
                        clientName = node.Node.Name;
                    }
                    else
                    {
                        clientName = string.Empty;
                    }
                }
                return clientName;
            }
        }
        #endregion

        #region OntologyName
        public string OntologyName
        {
            get
            {
                if (ontologyName == null)
                {
                    ontologyName = this._Ontology.Ontology.Name;
                }
                return ontologyName;
            }
        }
        #endregion

        public string ActionName
        {
            get
            {
                if (actionName == null)
                {
                    ActionState action;
                    if (!this._Ontology.Actions.TryGetValue(new Verb(this.Verb), out action))
                    {
                        throw new CoreException("意外的" + this.OntologyName + "动作码" + this.Verb);
                    }
                    actionName = action.Name;
                }
                return actionName;
            }
        }

        #region HumanInfo
        /// <summary>
        /// 命令信息，经过翻译的
        /// </summary>
        public string HumanInfo
        {
            get
            {
                if (_commandInfo == null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(this.ActionName);
                    sb.Append("：");
                    int l = sb.Length;
                    foreach (var item in InfoValueItems)
                    {
                        if (sb.Length != l)
                        {
                            sb.Append(";");
                        }
                        sb.Append(item.Element.Element.Name)
                            .Append("=>").Append(item.Element.TranslateValue(item.Value));
                    }
                    _commandInfo = sb.ToString();
                }
                return _commandInfo;
            }
            set
            {
                _commandInfo = value;
            }
        }
        #endregion

        private OntologyDescriptor _Ontology
        {
            get
            {
                if (ontology == null)
                {
                    if (!host.Ontologies.TryGetOntology(this.Ontology, out ontology))
                    {
                        throw new CoreException("意外的本体码" + this.Ontology);
                    }
                }
                return ontology;
            }
        }

        /// <summary>
        /// 根据传入的命令对象组装命令展示对象
        /// </summary>
        /// <param name="command"></param>
        private void Populate(MessageEntity command)
        {
            this.command = command;
            this.Id = command.Id;
            this.MessageID = command.MessageID;
            this.MessageType = command.MessageType.ToName();
            this.Verb = command.Verb.Code;
            this.InfoFormat = command.DataTuple.InfoFormat;
            this.InfoValue = command.DataTuple.ValueItems.InfoString;
            this.Ontology = command.Ontology;
            this.OrganizationCode = command.OrganizationCode;
            this.InfoID = command.DataTuple.IDItems.InfoString;
            this.LocalEntityID = command.LocalEntityID;
            this.ClientType = command.ClientType.ToName();
            this.ClientID = command.ClientID;
            this.ReceivedOn = command.ReceivedOn;
            this.TimeStamp = command.TimeStamp;
            this.CreateOn = command.CreateOn;
            this.UserName = command.UserName;
            this.StateCode = command.Status;
            this.ReasonPhrase = command.ReasonPhrase;
            this.Description = command.Description;
            this.Principal = command.UserName;
            this.EventSubjectCode = command.EventSubjectCode;
            this.EventSourceType = command.EventSourceType;
            this.PopulateCore(command);
        }

        #region private InfoValueItems
        private IList<InfoItem> InfoValueItems
        {
            get
            {
                if (_infoValueItems == null)
                {
                    OntologyDescriptor ontology;
                    if (!host.Ontologies.TryGetOntology(this.Ontology, out ontology))
                    {
                        return EmptyInfoValueItems;
                    }
                    _infoValueItems = new List<InfoItem>();
                    foreach (var item in this.command.DataTuple.ValueItems.Items)
                    {
                        _infoValueItems.Add(InfoItem.Create(ontology.Elements[item.Key], item.Value));
                    }
                }

                return _infoValueItems;
            }
            set
            {
                _infoValueItems = value;
            }
        }
        #endregion
    }
}

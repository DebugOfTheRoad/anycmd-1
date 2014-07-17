
namespace Anycmd.Host.EDI.MessageHandlers
{
    using Anycmd.EDI;
    using Anycmd.Repositories;
    using Commands;
    using DataContracts;
    using EDI.Handlers;
    using Entities;
    using Exceptions;
    using Hecp;
    using Info;
    using Messages;
    using Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Util;

    public class AddBatchCommandHandler : CommandHandler<AddBatchCommand>
    {
        private readonly AppHost host;

        public AddBatchCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(AddBatchCommand command)
        {
            var batchRepository = NodeHost.Instance.GetRequiredService<IRepository<Batch>>();
            OntologyDescriptor ontology;
            if (!NodeHost.Instance.Ontologies.TryGetOntology(command.Input.OntologyID, out ontology))
            {
                throw new ValidationException("非法的本体标识" + command.Input.OntologyID);
            }
            BatchType type;
            if (!command.Input.Type.TryParse(out type))
            {
                throw new ValidationException("意外的批类型" + command.Input.Type);
            }
            
            var entity = Batch.Create(command.Input);

            BatchDescriptor descriptor = new BatchDescriptor(entity);
            int pageSize = 1000;
            int pageIndex = 0;
            bool includeDescendants;
            if (entity.IncludeDescendants.HasValue)
            {
                includeDescendants = entity.IncludeDescendants.Value;
            }
            else
            {
                includeDescendants = false;
            }
            NodeDescriptor toNode = null;
            if (!NodeHost.Instance.Nodes.TryGetNodeByID(entity.NodeID.ToString(), out toNode))
            {
                throw new CoreException("意外的节点标识" + entity.NodeID);
            }

            string thisNodeID = NodeHost.Instance.Nodes.ThisNode.Node.Id.ToString();
            Verb actionCode;
            switch (descriptor.Type)
            {
                case BatchType.BuildCreateCommand:
                    actionCode = Verb.Create;
                    break;
                case BatchType.BuildUpdateCommand:
                    actionCode = Verb.Update;
                    break;
                case BatchType.BuildDeleteCommand:
                    actionCode = Verb.Delete;
                    break;
                default:
                    throw new CoreException("意外的批类型" + entity.Type);
            }
            var commandFactory = NodeHost.Instance.MessageProducer;
            IDataTuples entities = null;
            bool goOn = true;
            int count = 0;
            PagingInput pagingData = new PagingInput(pageIndex, pageSize, ontology.IncrementIDElement.Element.Code, "asc");
            var selectElements = new OrderedElementSet();
            selectElements.Add(ontology.IdElement);
            foreach (var item in ontology.Elements.Values.Where(a => a.Element.IsEnabled == 1))
            {
                if (toNode.IsCareforElement(item) || toNode.IsInfoIDElement(item))
                {
                    selectElements.Add(item);
                }
            }
            List<FilterData> filters = new List<FilterData>();
            if (ontology.Ontology.IsOrganizationalEntity && !string.IsNullOrEmpty(entity.OrganizationCode))
            {
                if (includeDescendants)
                {
                    filters.Add(FilterData.Like("ZZJGM", entity.OrganizationCode + "%"));
                }
                else
                {
                    filters.Add(FilterData.EQ("ZZJGM", entity.OrganizationCode));
                }
            }
            do
            {
                entities = ontology.EntityProvider.GetPlist(ontology, selectElements, filters, pagingData);
                if (entities != null && entities.Columns != null)
                {
                    int idIndex = -1;
                    for (int i = 0; i < entities.Columns.Count; i++)
                    {
                        if (entities.Columns[i] == ontology.IdElement)
                        {
                            idIndex = i;
                            break;
                        }
                    }
                    if (entities.Columns.Count == 0)
                    {
                        throw new CoreException("意外的查询列数");
                    }
                    if (idIndex == -1)
                    {
                        throw new CoreException("未查询得到实体标识列");
                    }
                    List<MessageEntity> products = new List<MessageEntity>();
                    foreach (var item in entities.Tuples)
                    {
                        InfoItem[] idItems = new InfoItem[] { InfoItem.Create(ontology.IdElement, item[idIndex].ToString()) };
                        InfoItem[] valueItems = new InfoItem[entities.Columns.Count - 1];
                        for (int i = 0; i < entities.Columns.Count; i++)
                        {
                            if (i < idIndex)
                            {
                                valueItems[i] = InfoItem.Create(entities.Columns[i], item[i].ToString());
                            }
                            else if (i > idIndex)
                            {
                                valueItems[i - 1] = InfoItem.Create(entities.Columns[i], item[i].ToString());
                            }
                        }
                        var commandContext = new MessageContext(
                                new MessageRecord(
                                    MessageTypeKind.Received,
                                    Guid.NewGuid(),
                                    DataItemsTuple.Create(idItems, valueItems, null, "json"))
                                    {
                                        Verb = actionCode,
                                        ClientID = thisNodeID,
                                        ClientType = ClientType.Node,
                                        TimeStamp = DateTime.Now,
                                        CreateOn = DateTime.Now,
                                        Description = entity.Type,
                                        LocalEntityID = item[idIndex].ToString(),
                                        Ontology = ontology.Ontology.Code,
                                        OrganizationCode = null,// 如果按照组织结构分片分发命令的话则组织结构为空的分发命令是由专门的分发器分发的
                                        MessageID = Guid.NewGuid().ToString(),
                                        MessageType = MessageType.Action,
                                        ReasonPhrase = "Ok",
                                        Status = 200,
                                        EventSourceType = string.Empty,
                                        EventSubjectCode = string.Empty,
                                        UserName = NodeHost.Instance.AppHost.User.Worker.Id.ToString(),
                                        IsDumb = false,
                                        ReceivedOn = DateTime.Now,
                                        Version = ApiVersion.V1.ToName()
                                    });
                        products.AddRange(commandFactory.Produce(new MessageTuple(commandContext, valueItems), toNode));
                    }
                    ontology.MessageProvider.SaveCommands(ontology, products.ToArray());
                    count = count + products.Count;
                    pagingData.pageIndex++;// 注意这里不要引入bug。
                }
                if (entities == null || entities.Tuples.Length == 0)
                {
                    goOn = false;
                }
            } while (goOn);
            entity.Total = count;

            batchRepository.Add(entity);
            batchRepository.Context.Commit();

            host.PublishEvent(new BatchAddedEvent(entity));
            host.CommitEventBus();
        }

        #region MessageRecord
        public class MessageRecord : MessageBase
        {
            public MessageRecord(MessageTypeKind type, Guid id, DataItemsTuple dataTuple)
                : base(type, id, dataTuple)
            {

            }
            #region Public Properties
            /// <summary>
            /// 协议版本号
            /// </summary>
            public new string Version
            {
                get { return base.Version; }
                protected internal set
                {
                    base.Version = value;
                }
            }

            /// <summary>
            /// 命令类型
            /// </summary>
            public new MessageTypeKind CommandType
            {
                get { return base.CommandType; }
            }

            /// <summary>
            /// 命令标识
            /// </summary>
            public new Guid Id
            {
                get { return base.Id; }
                set
                {
                    base.Id = value;
                }
            }

            /// <summary>
            /// 是否是哑的
            /// </summary>
            public new bool IsDumb
            {
                get { return base.IsDumb; }
                protected internal set
                {
                    base.IsDumb = value;
                }
            }

            /// <summary>
            /// 请求类型
            /// </summary>
            public new MessageType MessageType
            {
                get { return base.MessageType; }
                protected internal set
                {
                    base.MessageType = value;
                }
            }

            /// <summary>
            /// 远端命令标识
            /// </summary>
            public new string MessageID
            {
                get { return base.MessageID; }
                protected internal set
                {
                    base.MessageID = value;
                }
            }

            public new string RelatesTo
            {
                get { return base.RelatesTo; }
                protected internal set
                {
                    base.RelatesTo = value;
                }
            }

            public new string To
            {
                get { return base.To; }
                protected internal set
                {
                    base.To = value;
                }
            }

            public new string SessionID
            {
                get { return base.SessionID; }
                protected internal set
                {
                    base.SessionID = value;
                }
            }

            public new FromData From
            {
                get { return base.From; }
                protected internal set
                {
                    base.From = value;
                }
            }

            /// <summary>
            /// 本体实体标识
            /// </summary>
            public new string LocalEntityID
            {
                get { return base.LocalEntityID; }
                protected internal set
                {
                    base.LocalEntityID = value;
                }
            }

            /// <summary>
            /// 本地组织结构码
            /// </summary>
            public new string OrganizationCode
            {
                get { return base.OrganizationCode; }
                protected internal set
                {
                    base.OrganizationCode = value;
                }
            }

            /// <summary>
            /// 客户端类型
            /// </summary>
            public new ClientType ClientType
            {
                get { return base.ClientType; }
                protected internal set
                {
                    base.ClientType = value;
                }
            }

            /// <summary>
            /// 本体码
            /// </summary>
            public new string Ontology
            {
                get { return base.Ontology; }
                protected internal set
                {
                    base.Ontology = value;
                }
            }

            /// <summary>
            /// 动作类型
            /// </summary>
            public new Verb Verb
            {
                get { return base.Verb; }
                protected internal set
                {
                    base.Verb = value;
                }
            }

            /// <summary>
            /// 节点标识
            /// </summary>
            public new string ClientID
            {
                get { return base.ClientID; }
                protected internal set
                {
                    base.ClientID = value;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public new DateTime ReceivedOn
            {
                get { return base.ReceivedOn; }
                protected internal set
                {
                    base.ReceivedOn = value;
                }
            }

            /// <summary>
            /// 命令时间戳
            /// </summary>
            public new DateTime TimeStamp
            {
                get { return base.TimeStamp; }
                protected internal set
                {
                    base.TimeStamp = value;
                }
            }

            /// <summary>
            /// 发送时间
            /// </summary>
            public new DateTime CreateOn
            {
                get { return base.CreateOn; }
                protected internal set
                {
                    base.CreateOn = value;
                }
            }

            /// <summary>
            /// 命令状态码
            /// </summary>
            public new int Status
            {
                get { return base.Status; }
                protected internal set
                {
                    base.Status = value;
                }
            }

            /// <summary>
            /// 原因短语
            /// </summary>
            public new string ReasonPhrase
            {
                get { return base.ReasonPhrase; }
                protected internal set
                {
                    base.ReasonPhrase = value;
                }
            }

            /// <summary>
            /// 命令状态描述
            /// </summary>
            public new string Description
            {
                get { return base.Description; }
                protected internal set
                {
                    base.Description = value;
                }
            }

            /// <summary>
            /// 事件主题
            /// </summary>
            public new string EventSubjectCode
            {
                get { return base.EventSubjectCode; }
                protected internal set
                {
                    base.EventSubjectCode = value;
                }
            }

            /// <summary>
            /// 其值为枚举字符串
            /// </summary>
            public new string EventSourceType
            {
                get { return base.EventSourceType; }
                protected internal set
                {
                    base.EventSourceType = value;
                }
            }

            /// <summary>
            /// 发起人
            /// </summary>
            public new string UserName
            {
                get { return base.UserName; }
                protected internal set
                {
                    base.UserName = value;
                }
            }

            /// <summary>
            /// 数据项集合对
            /// </summary>
            public new DataItemsTuple DataTuple
            {
                get { return base.DataTuple; }
            }
            #endregion
        }
        #endregion
    }
}

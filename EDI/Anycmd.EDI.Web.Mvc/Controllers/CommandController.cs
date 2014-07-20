
namespace Anycmd.EDI.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using DataContracts;
    using Exceptions;
    using Host.EDI;
    using Host.EDI.Handlers;
    using Host.EDI.Hecp;
    using Host.EDI.Info;
    using MessageViewModels;
    using MiniUI;
    using ServiceModel.Operations;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 命令型视图控制器<see cref="MessageEntity"/>
    /// </summary>
    public class CommandController : AnycmdController
    {
        #region Views
        /// <summary>
        /// 命令详细信息
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("命令详细信息")]
        public ViewResultBase Details()
        {
            return ViewResult();
        }

        /// <summary>
        /// 已接收的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("已接收的命令")]
        public ViewResultBase ReceivedMessage()
        {
            return ViewResult();
        }

        /// <summary>
        /// 接收失败的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("接收失败的命令")]
        public ViewResultBase UnacceptedMessage()
        {
            return ViewResult();
        }

        /// <summary>
        /// 执行失败的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("执行失败的命令")]
        public ViewResultBase HandleFailingCommand()
        {
            return ViewResult();
        }

        /// <summary>
        /// 已执行的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("已执行的命令")]
        public ViewResultBase HandledCommand()
        {
            return ViewResult();
        }

        /// <summary>
        /// 待分发的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("待分发的命令")]
        public ViewResultBase DistributeMessage()
        {
            return ViewResult();
        }

        /// <summary>
        /// 分发失败的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分发失败的命令")]
        public ViewResultBase DistributeFailingMessage()
        {
            return ViewResult();
        }

        /// <summary>
        /// 已分发的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("已分发的命令")]
        public ViewResultBase DistributedMessage()
        {
            return ViewResult();
        }

        /// <summary>
        /// 本地事件
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("本地事件")]
        public ViewResultBase LocalEvent()
        {
            return ViewResult();
        }

        /// <summary>
        /// 客户端事件
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("客户端事件")]
        public ViewResultBase ClientEvent()
        {
            return ViewResult();
        }

        #endregion

        #region EntityViewResults
        /// <summary>
        /// 已成功执行的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("已成功执行的命令")]
        public ViewResultBase EntityHandledCommands()
        {
            return ViewResult();
        }

        /// <summary>
        /// 执行失败的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("执行失败的命令")]
        public ViewResultBase EntityHandleFailingCommands()
        {
            return ViewResult();
        }

        /// <summary>
        /// 已成功接收的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("已成功接收的命令")]
        public ViewResultBase EntityReceivedMessages()
        {
            return ViewResult();
        }

        /// <summary>
        /// 待分发的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("待分发的命令")]
        public ViewResultBase EntityDistributeMessages()
        {
            return ViewResult();
        }

        /// <summary>
        /// 已分发的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("已分发的命令")]
        public ViewResultBase EntityDistributedMessages()
        {
            return ViewResult();
        }

        /// <summary>
        /// 分发失败的命令
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分发失败的命令")]
        public ViewResultBase EntityDistributeFailingMessages()
        {
            return ViewResult();
        }

        /// <summary>
        /// 本地事件
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("本地事件")]
        public ViewResultBase EntityLocalEvents()
        {
            return ViewResult();
        }

        /// <summary>
        /// 客户端事件
        /// </summary>
        /// <param name="isInner"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("客户端事件")]
        public ViewResultBase EntityClientEvents()
        {
            return ViewResult();
        }
        #endregion

        #region AuditApproved
        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("审核通过")]
        [HttpPost]
        public ActionResult AuditApproved(string ontologyCode, string id)
        {
            var response = new ResponseData { id = id, success = true };
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyCode, out ontology))
            {
                throw new ValidationException("非法的本体码");
            }
            string[] ids = id.Split(',');
            var localEventIDs = new Guid[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                Guid tmp;
                if (Guid.TryParse(ids[i], out tmp))
                {
                    localEventIDs[i] = tmp;
                }
                else
                {
                    throw new ValidationException("意外的本地事件标识" + ids[i]);
                }
            }
            foreach (var localEventID in localEventIDs)
            {
                MessageEntity evnt = ontology.MessageProvider.GetCommand(MessageTypeKind.LocalEvent, ontology, localEventID);
                if (evnt != null)
                {
                    if (evnt.Status == (int)Status.ToAudit
                    && evnt.EventSourceType.Equals("Command", StringComparison.OrdinalIgnoreCase))
                    {
                        var node = Host.Nodes.ThisNode;
                        var ticks = DateTime.UtcNow.Ticks;
                        var cmd = new Message()
                        {
                            Version = ApiVersion.V1.ToName(),
                            IsDumb = false,
                            MessageType = MessageType.Event.ToName(),
                            Credential = new CredentialData
                            {
                                ClientType = ClientType.Node.ToName(),
                                UserType = UserType.None.ToName(),
                                CredentialType = CredentialType.Token.ToName(),
                                ClientID = node.Node.PublicKey.ToString(),
                                UserName = evnt.UserName,// UserName
                                Password = TokenObject.Token(node.Node.Id.ToString(), ticks, node.Node.SecretKey),
                                Ticks = ticks
                            },
                            TimeStamp = DateTime.UtcNow.Ticks,
                            MessageID = evnt.Id.ToString(),
                            Verb = evnt.Verb.Code,
                            Ontology = evnt.Ontology,
                            Body = new BodyData(new KeyValue[] { new KeyValue("Id", evnt.LocalEntityID) }, evnt.DataTuple.ValueItems.Items.ToDto())
                            {
                                Event = new EventData
                                {
                                    Status = (int)Status.AuditApproved,
                                    ReasonPhrase = Status.AuditApproved.ToName(),
                                    SourceType = evnt.EventSourceType,
                                    Subject = evnt.EventSubjectCode
                                }
                            }
                        };
                        var result = AnyMessage.Create(HecpRequest.Create(cmd), Host.Nodes.ThisNode).Response();
                        if (result.Body.Event.Status == (int)Status.NotExist)
                        {
                            ontology.MessageProvider.DeleteCommand(MessageTypeKind.LocalEvent, ontology, evnt.Id, evnt.IsDumb);
                        }
                        else
                        {
                            if (result.Body.Event.Status < 200 || result.Body.Event.Status >= 400)
                            {
                                response.success = false;
                                response.msg = result.Body.Event.Description;
                                response.Warning();
                            }
                        }
                    }
                }
                else
                {
                    response.success = false;
                    response.msg = "给定标识的本地事件不存在";
                    response.Warning();
                }
            }

            return this.JsonResult(response);
        }
        #endregion

        #region AuditUnapproved
        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("审核不通过")]
        [HttpPost]
        public ActionResult AuditUnapproved(string ontologyCode, string id)
        {
            var response = new ResponseData { id = id, success = true };
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyCode, out ontology))
            {
                throw new ValidationException("非法的本体码");
            }
            string[] ids = id.Split(',');
            var localEventIDs = new Guid[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                Guid tmp;
                if (Guid.TryParse(ids[i], out tmp))
                {
                    localEventIDs[i] = tmp;
                }
                else
                {
                    throw new ValidationException("意外的本地事件标识" + ids[i]);
                }
            }
            foreach (var localEventID in localEventIDs)
            {
                MessageEntity evnt = ontology.MessageProvider.GetCommand(MessageTypeKind.LocalEvent, ontology, localEventID);
                if (evnt != null)
                {
                    if (evnt.Status == (int)Status.ToAudit
                    && evnt.EventSourceType.Equals("Command", StringComparison.OrdinalIgnoreCase))
                    {
                        var node = Host.Nodes.ThisNode;
                        var ticks = DateTime.UtcNow.Ticks;
                        var cmd = new Message()
                        {
                            Version = ApiVersion.V1.ToName(),
                            IsDumb = false,
                            MessageType = MessageType.Event.ToName(),
                            Credential = new CredentialData
                            {
                                ClientType = ClientType.Node.ToName(),
                                UserType = UserType.None.ToName(),
                                CredentialType = CredentialType.Token.ToName(),
                                ClientID = node.Node.Id.ToString(),
                                UserName = evnt.UserName,// UserName
                                Password = TokenObject.Token(node.Node.Id.ToString(), ticks, node.Node.SecretKey),
                                Ticks = ticks
                            },
                            TimeStamp = DateTime.UtcNow.Ticks,
                            MessageID = evnt.Id.ToString(),
                            Verb = evnt.Verb.Code,
                            Ontology = evnt.Ontology,
                            Body = new BodyData(new KeyValue[] { new KeyValue("Id", evnt.LocalEntityID) }, evnt.DataTuple.ValueItems.Items.ToDto())
                            {
                                Event = new EventData
                                {
                                    Status = (int)Status.AuditUnapproved,
                                    ReasonPhrase = Status.AuditUnapproved.ToName(),
                                    SourceType = evnt.EventSourceType,
                                    Subject = evnt.EventSubjectCode
                                }
                            }
                        };
                        var result = AnyMessage.Create(HecpRequest.Create(cmd), Host.Nodes.ThisNode).Response();
                        if (result.Body.Event.Status == (int)Status.NotExist)
                        {
                            ontology.MessageProvider.DeleteCommand(MessageTypeKind.LocalEvent, ontology, evnt.Id, evnt.IsDumb);
                        }
                        else
                        {
                            if ((result.Body.Event.Status < 200 || result.Body.Event.Status >= 400) && result.Body.Event.Status != (int)Status.AuditUnapproved)
                            {
                                response.success = false;
                                response.msg = result.Body.Event.Description;
                                response.Warning();
                            }
                        }
                    }
                }
            }

            return this.JsonResult(response);
        }
        #endregion

        #region GetInfo
        /// <summary>
        /// 根据命令ID获取给定类型的命令的详细信息
        /// </summary>
        /// <param name="ontologyCode"></param>
        /// <param name="id"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("根据命令ID获取给定类型的命令的详细信息")]
        public ActionResult GetInfo(string ontologyCode, Guid? id, string commandType)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("命令标识不能为空");
            }
            if (string.IsNullOrEmpty(commandType))
            {
                throw new ValidationException("命令类型不能为空");
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(ontologyCode, out ontology))
            {
                throw new ValidationException("非法的本体码");
            }
            MessageTypeKind commandTypeEnum;
            if (!commandType.TryParse(out commandTypeEnum))
            {
                throw new ValidationException("非法的命令类型" + commandType);
            }
            MessageTr data = null;
            switch (commandTypeEnum)
            {
                case MessageTypeKind.Invalid:
                    throw new ValidationException("非法的命令类型");
                case MessageTypeKind.AnyCommand:
                    throw new ValidationException("AnyCommand不是实体命令类型");
                case MessageTypeKind.Received:
                case MessageTypeKind.Unaccepted:
                case MessageTypeKind.Executed:
                case MessageTypeKind.ExecuteFailing:
                case MessageTypeKind.Distribute:
                case MessageTypeKind.Distributed:
                case MessageTypeKind.DistributeFailing:
                case MessageTypeKind.LocalEvent:
                case MessageTypeKind.ClientEvent:
                    data = ontology.MessageProvider.GetCommandInfo(commandTypeEnum, ontology, id.Value);
                    break;
                default:
                    throw new ValidationException("非法的命令类型" + commandType);
            }
            return this.JsonResult(data);
        }
        #endregion

        #region GetPlist
        /// <summary>
        /// 分页获取命令
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [By("xuexs")]
        [Description("分页获取命令")]
        public ActionResult GetPlist(GetPlistMessages requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            OntologyDescriptor ontology;
            if (!Host.Ontologies.TryGetOntology(requestModel.ontologyCode, out ontology))
            {
                throw new ValidationException("非法的本体码");
            }
            MessageTypeKind commandTypeEnum;
            if (!requestModel.commandType.TryParse(out commandTypeEnum))
            {
                throw new ValidationException("非法的命令类型" + requestModel.commandType);
            }
            long total;
            IList<MessageTr> list = null;
            switch (commandTypeEnum)
            {
                case MessageTypeKind.Invalid:
                    throw new ValidationException("非法的命令类型");
                case MessageTypeKind.AnyCommand:
                    throw new ValidationException("AnyCommand不是实体命令类型");
                case MessageTypeKind.Received:
                case MessageTypeKind.Unaccepted:
                case MessageTypeKind.Executed:
                case MessageTypeKind.ExecuteFailing:
                case MessageTypeKind.Distribute:
                case MessageTypeKind.Distributed:
                case MessageTypeKind.DistributeFailing:
                case MessageTypeKind.LocalEvent:
                case MessageTypeKind.ClientEvent:
                    list = ontology.MessageProvider.GetPlistCommandTrs(
                        commandTypeEnum, ontology, requestModel.organizationCode,
                        requestModel.actionCode, requestModel.nodeID, requestModel.entityID,
                        requestModel.pageIndex.Value, requestModel.pageSize.Value,
                        requestModel.sortField, requestModel.sortOrder, out total);
                    break;
                default:
                    throw new ValidationException("非法的命令类型" + requestModel.commandType);
            }
            var data = new MiniGrid<MessageTr> { data = list, total = total };

            return this.JsonResult(data);
        }
        #endregion
    }
}

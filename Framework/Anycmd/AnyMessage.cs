﻿
namespace Anycmd
{
    using Anycmd.Host.EDI;
    using Anycmd.Host.EDI.Handlers;
    using Anycmd.Host.EDI.Handlers.Distribute;
    using Anycmd.Host.EDI.Info;
    using DataContracts;
    using Exceptions;
    using Host.EDI.Hecp;
    using System;
    using Util;

    /// <summary>
    /// 服务端命令
    /// </summary>
    public sealed class AnyMessage : MessageBase
    {
        private readonly NodeDescriptor responseNode;

        #region Ctor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataTuple"></param>
        /// <param name = "responseNode"></param>
        private AnyMessage(MessageTypeKind type, DataItemsTuple dataTuple, NodeDescriptor responseNode)
            : this(type, Guid.NewGuid(), dataTuple, responseNode)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dataTuple">数据项集合对</param>
        /// <param name="id">信息标识</param>
        /// <param name = "responseNode"></param>
        private AnyMessage(MessageTypeKind type, Guid id, DataItemsTuple dataTuple, NodeDescriptor responseNode)
            : base(type, id, dataTuple)
        {
            this.responseNode = responseNode;
        }
        #endregion

        /// <summary>
        /// 创建给定的节点的命令
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseNode"></param>
        /// <returns></returns>
        public static AnyMessage Create(HecpRequest request, NodeDescriptor responseNode)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (responseNode == null)
            {
                throw new ArgumentNullException("responseNode");
            }
            string clientID = string.Empty;
            var credential = request.Credential;
            if (credential != null)
            {
                switch (credential.ClientType)
                {
                    case ClientType.Undefined:
                        break;
                    case ClientType.Node:
                        NodeDescriptor requester;
                        if (NodeHost.Instance.Nodes.TryGetNodeByPublicKey(credential.ClientID, out requester))
                        {
                            clientID = requester.Id.ToString();
                        }
                        break;
                    case ClientType.App:
                        break;
                    case ClientType.Monitor:
                        break;
                    default:
                        break;
                }
            }
            var dataTuple = DataItemsTuple.Create(
                request.InfoID,
                request.InfoValue, request.QueryList, HostConfig.Instance.InfoFormat);
            MessageType requestType;
            request.MessageType.TryParse(out requestType);
            return new AnyMessage(MessageTypeKind.AnyCommand, Guid.NewGuid(), dataTuple, responseNode)
            {
                Version = request.Version,
                IsDumb = request.IsDumb,
                Ontology = request.Ontology,
                ReceivedOn = DateTime.Now,
                CreateOn = DateTime.Now,
                MessageID = request.MessageID,
                ClientID = clientID,
                Verb = request.Verb,
                MessageType = requestType,
                ClientType = credential.ClientType,
                TimeStamp = SystemTime.ParseUtcTicksToLocalTime(request.TimeStamp),
                ReasonPhrase = request.EventReasonPhrase,
                Status = request.EventStatus,
                UserName = request.Credential.UserName,
                EventSourceType = request.EventSourceType,
                EventSubjectCode = request.EventSubject,
                LocalEntityID = null,
                Description = null,
                OrganizationCode = null,
                From = request.From,
                RelatesTo = request.RelatesTo,
                To = request.To,
                SessionID = request.SessionID
            };
        }

        /// <summary>
        /// 本地执行。
        /// <remarks>如果命令的响应节点不是本节点则不能调用本方法。本方法为管道过滤器等第三方插件提供了一个绕开网络执行命令的捷径。</remarks>
        /// </summary>
        /// <returns></returns>
        public IMessageDto Response()
        {
            if (responseNode != NodeHost.Instance.Nodes.ThisNode)
            {
                throw new CoreException("当前命令的响应节点不是本节点，不支持调用本方法。该方法设计用于绕过网络通信供服务节点调试使用。");
            }
            var context = new MessageContext(this);
            MessageHandler.Instance.Response(context);

            return context.Result;
        }

        /// <summary>
        /// 通过网络传输命令到服务端执行。
        /// <remarks>即使命令的响应节点是本节点该方法依旧通过网络传送命令。</remarks>
        /// </summary>
        /// <returns></returns>
        public IMessageDto Request()
        {
            var context = new DistributeContext(this, this.responseNode);
            MessageRequester.Instance.Request(context);

            return context.Result;
        }
    }
}

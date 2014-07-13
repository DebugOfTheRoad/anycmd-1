
namespace Anycmd.EDI.Client
{
    using DataContracts;
    using Util;
    using Exceptions;
    using System;
    using Anycmd.Host.EDI;
    using Anycmd.Host.EDI.Hecp;

    /// <summary>
    /// 扩展命令数据传输对象接口。提供转移命令的方法，用以为客户端程序员隔离复杂的命令知识。
    /// </summary>
    public static class MessageExtension
    {
        /// <summary>
        /// 此方法从节点上下文中读取自我节点。调用本方法需要启用数据交换配置系统。
        /// </summary>
        /// <param name="cmdDto"></param>
        /// <returns></returns>
        public static IMessageDto RequestThisNode(this IMessageDto cmdDto)
        {
            return RequestNode(cmdDto, NodeHost.Instance.Nodes.ThisNode);
        }

        /// <summary>
        /// 此方法从节点上下文中读取中心节点。调用本方法需要启用数据交换配置系统。
        /// </summary>
        /// <param name="cmdDto"></param>
        /// <returns></returns>
        public static IMessageDto RequestCenterNode(this IMessageDto cmdDto)
        {
            return RequestNode(cmdDto, NodeHost.Instance.Nodes.CenterNode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdDto"></param>
        /// <param name="toNode"></param>
        /// <returns></returns>
        public static IMessageDto RequestNode(this IMessageDto cmdDto, NodeDescriptor toNode)
        {
            if (cmdDto == null)
            {
                throw new ArgumentNullException("cmdDto");
            }
            if (toNode == null)
            {
                throw new ArgumentNullException("toNode");
            }
            if (cmdDto.Credential == null)
            {
                throw new CoreException("非法状态的命令，没有设置证书");
            }
            if (string.IsNullOrEmpty(cmdDto.Credential.Password))
            {
                CredentialType credentialType;
                if (!cmdDto.Credential.CredentialType.TryParse(out credentialType))
                {
                    throw new CoreException("意外的证书类型" + cmdDto.Credential.CredentialType);
                }
                ClientType clientType;
                if (!cmdDto.Credential.ClientType.TryParse(out clientType))
                {
                    throw new CoreException("意外的客户端类型" + cmdDto.Credential.ClientType);
                }
                switch (clientType)
                {
                    case ClientType.Undefined:
                        break;
                    case ClientType.Node:
                        NodeDescriptor clientNode;
                        if (!NodeHost.Instance.Nodes.TryGetNodeByPublicKey(cmdDto.Credential.ClientID, out clientNode))
                        {
                            throw new CoreException("意外的客户节点标识" + cmdDto.Credential.ClientID);
                        }
                        switch (credentialType)
                        {
                            case CredentialType.Undefined:
                                break;
                            case CredentialType.Token:
                                cmdDto.Credential.Password = TokenObject.Token(cmdDto.Credential.ClientID, cmdDto.Credential.Ticks, clientNode.Node.SecretKey);
                                break;
                            case CredentialType.Signature:
                                cmdDto.Credential.Password = Signature.Sign(cmdDto.ToOrignalString(cmdDto.Credential), clientNode.Node.SecretKey);
                                break;
                            case CredentialType.OAuth:
                                break;
                            default:
                                break;
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
            return cmdDto.ToAnyCommand(toNode).Request();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdDto"></param>
        /// <param name="responseNode"></param>
        /// <returns></returns>
        public static AnyMessage ToAnyCommand(this IMessageDto cmdDto, NodeDescriptor responseNode)
        {
            if (cmdDto == null)
            {
                throw new ArgumentNullException("cmdDto");
            }
            if (responseNode == null)
            {
                throw new ArgumentNullException("responseNode");
            }
            return AnyMessage.Create(HecpRequest.Create(cmdDto), responseNode);
        }
    }
}

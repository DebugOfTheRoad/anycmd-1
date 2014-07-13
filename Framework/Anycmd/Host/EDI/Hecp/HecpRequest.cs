
namespace Anycmd.Host.EDI.Hecp {
    using DataContracts;
    using Exceptions;
    using Info;
    using System;
    using System.Linq;

    /// <summary>
    /// 包含单条命令的EDI请求
    /// </summary>
    public sealed class HecpRequest {
        private readonly Verb verb;
        private readonly string eventSourceType;
        private readonly string eventSubject;
        private readonly int eventStatus;
        private readonly string eventReasonPhrase;
        private readonly IMessageDto message;
        private readonly DataItem[] infoID;
        private readonly DataItem[] infoValue;
        private readonly string[] queryList;

        private HecpRequest() {
        }

        private HecpRequest(IMessageDto cmdDto) {
            if (cmdDto == null) {
                throw new ArgumentNullException("cmdDto");
            }
            if (cmdDto.Body == null) {
                throw new CoreException();
            }
            this.message = cmdDto;
            if (cmdDto.Body.InfoID == null) {
                infoID = new DataItem[0];
            }
            if (cmdDto.Body.InfoValue == null) {
                infoValue = new DataItem[0];
            }
            if (cmdDto.Body.InfoID != null) {
                infoID = cmdDto.Body.InfoID.Where(a => a != null).Select(a => new DataItem(a.Key, a.Value)).ToArray();
            }
            if (cmdDto.Body.InfoValue != null) {
                infoValue = cmdDto.Body.InfoValue.Where(a => a != null).Select(a => new DataItem(a.Key, a.Value)).ToArray();
            }
            this.queryList = cmdDto.Body.QueryList;
            this.Credential = new CredentialObject(cmdDto.Credential);
            this.verb = new Verb(cmdDto.Verb);
            if (cmdDto.Body.Event != null) {
                eventSourceType = cmdDto.Body.Event.SourceType;
                eventSubject = cmdDto.Body.Event.Subject;
                eventStatus = cmdDto.Body.Event.Status;
                eventReasonPhrase = cmdDto.Body.Event.ReasonPhrase;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdDto"></param>
        /// <returns></returns>
        public static HecpRequest Create(IMessageDto cmdDto) {
            return cmdDto == null ? null : new HecpRequest(cmdDto);
        }

        /// <summary>
        /// 证书
        /// </summary>
        public CredentialObject Credential { get; private set; }

        /// <summary>
        /// ApiVersion
        /// </summary>
        public string Version { get { return this.message.Version; } }

        /// <summary>
        /// 是否是哑的
        /// </summary>
        public bool IsDumb { get { return this.message.IsDumb; } }

        /// <summary>
        /// 
        /// </summary>
        public string MessageType { get { return this.message.MessageType; } }

        /// <summary>
        /// 动作类型
        /// </summary>
        public Verb Verb { get { return this.verb; } }

        /// <summary>
        /// 本体码
        /// </summary>
        public string Ontology { get { return this.message.Ontology; } }

        /// <summary>
        /// 客户端生成的时间戳。根据消息类型的不同其为Event时间戳或Command时间戳或Action时间戳。
        /// <remarks>
        /// 注意：Message是来自客户端的消息，因此TimeStamp指的是客户端传入的时间戳。
        /// 对于Event该时间是事件在客户端发生的时间而对于Action和Command该时间戳的意义由客户端自由定义
        /// </remarks>
        /// </summary>
        public Int64 TimeStamp { get { return this.message.TimeStamp; } }

        /// <summary>
        /// 本地请求标识
        /// </summary>
        public string MessageID { get { return this.message.MessageID; } }

        /// <summary>
        /// 
        /// </summary>
        public FromData From { get { return this.message.From; } }

        /// <summary>
        /// 
        /// </summary>
        public string RelatesTo { get { return this.message.RelatesTo; } }

        /// <summary>
        /// 
        /// </summary>
        public string SessionID { get { return this.message.SessionID; } }

        /// <summary>
        /// 
        /// </summary>
        public string To { get { return this.message.To; } }

        /// <summary>
        /// 事件主题码
        /// </summary>
        public string EventSubject { get { return eventSubject; } }

        /// <summary>
        /// 事件源类型
        /// </summary>
        public string EventSourceType { get { return eventSourceType; } }

        /// <summary>
        /// 事件状态码
        /// </summary>
        public int EventStatus { get { return eventStatus; } }

        /// <summary>
        /// 事件原因短语
        /// </summary>
        public string EventReasonPhrase { get { return eventReasonPhrase; } }

        /// <summary>
        /// 信息标识
        /// </summary>
        public DataItem[] InfoID { get { return infoID; } }

        /// <summary>
        /// 信息值
        /// </summary>
        public DataItem[] InfoValue { get { return infoValue; } }

        public string[] QueryList { get { return queryList; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        public string ToOrignalString(ICredentialData credential) {
            return this.message.ToOrignalString(credential);
        }
    }
}

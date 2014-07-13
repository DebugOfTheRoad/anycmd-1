
namespace Anycmd.Host.EDI {
    using Anycmd.Host.EDI.Handlers;
    using DataContracts;
    using Info;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Util;

    /// <summary>
    /// 命令处理结果
    /// </summary>
    public sealed class QueryResult : IMessageDto {
        private readonly IMessage request;
        private CredentialData credential = new CredentialData();
        private readonly EventData evnt = new EventData {
            SourceType = EventSourceType.Command.ToName(),
            Subject = "Response",
            Status = 500,
            ReasonPhrase = "InternalServerError",
            Description = "服务逻辑异常"
        };

        /// <summary>
        /// 构造命令处理结果对象。
        /// </summary>
        internal QueryResult(IMessage request) {
            if (request == null) {
                throw new ArgumentNullException("request");
            }
            this.request = request;
            this.Version = request.Version;
            this.IsDumb = request.IsDumb;
            this.MessageType = "Event";
            this.Ontology = request.Ontology;
            this.TimeStamp = DateTime.UtcNow.Ticks;
            this.MessageID = request.MessageID;
            this.credential = new CredentialData();
            this.ResultDataItems = new List<DataItem>();
        }

        /// <summary>
        /// 当请求命令的Verb为get时有值。
        /// </summary>
        internal List<DataItem> ResultDataItems { get; set; }

        /// <summary>
        /// 命令在服务端的管道栈
        /// </summary>
        public string StackTrace { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccess {
            get {
                int value = (int)this.evnt.Status;
                return value >= 200 && value < 300;
            }
        }

        /// <summary>
        /// 查看或设置命令响应流程的关闭状态。设置为false以跳出命令响应过滤器的应用。
        /// </summary>
        public bool IsClosed { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDumb { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string MessageType { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Verb { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string Ontology { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public long TimeStamp { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public string MessageID { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        public int Status {
            get {
                return this.evnt.Status;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ReasonPhrase {
            get {
                return this.evnt.ReasonPhrase;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description {
            get {
                return this.evnt.Description;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RelatesTo {
            get { return this.request.RelatesTo; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string To {
            get { return this.request.To; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SessionID {
            get { return this.request.SessionID; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FromData From {
            get { return this.request.From; }
        }

        /// <summary>
        /// 
        /// </summary>
        CredentialData IMessageDto.Credential {
            get {
                return credential;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        BodyData IMessageDto.Body {
            get {
                return new BodyData(
                    request.DataTuple.IDItems.Items.Select(a => new KeyValue(a.Key, a.Value)).ToArray(),
                    this.ResultDataItems.Select(a => new KeyValue(a.Key, a.Value)).ToArray()) {
                        Event = this.evnt
                    };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="reasonPhrase"></param>
        /// <param name="description"></param>
        internal void UpdateStatus(Status status, string description) {
            this.evnt.Status = (int)status;
            this.evnt.ReasonPhrase = status.ToName();
            this.evnt.Description = description;
        }
    }
}


namespace Anycmd.Host.EDI.Hecp {
	using DataContracts;
	using System;

	/// <summary>
	/// Hecp响应模型
	/// </summary>
	public sealed class HecpResponse : IMessageDto {
		private static readonly string hostName = System.Net.Dns.GetHostName();
		private string serverID;
		private Int64 serverTicks = DateTime.UtcNow.Ticks;
		private CredentialData credential;
		private BodyData body;

		/// <summary>
		/// 
		/// </summary>
		private HecpResponse() {
			this.MessageType = "Event";
			this.body = new BodyData();
			this.credential = new CredentialData();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestID"></param>
		private HecpResponse(string requestID)
			: this() {
			this.MessageID = requestID;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="requestID"></param>
		/// <returns></returns>
		public static HecpResponse Create(string requestID) {
			return new HecpResponse(requestID);
		}

		/// <summary>
		/// 
		/// </summary>
		public string MessageID { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string RelatesTo { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string To { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string SessionID { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public FromData From { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsSuccess {
			get {
				return this.body.Event.Status >= 200 && this.body.Event.Status < 300;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public Int64 ServerTicks {
			get { return serverTicks; }
			private set { serverTicks = value; }
		}

		/// <summary>
		/// 服务器标识
		/// </summary>
		public string ServerID {
			get {
				return serverID ?? hostName;
			}
			private set {
				serverID = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string StackTrace { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public bool IsClosed { get; private set; }

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
		public string MessageType { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Verb { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Ontology { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public long TimeStamp { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public BodyData Body {
			get {
				return body;
			}
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
		/// <param name="status"></param>
		/// <param name="reasonPhrase"></param>
		/// <param name="description"></param>
		internal void UpdateStatus(Status status, string description) {
			this.Body.Event.Status = (int)status;
			this.Body.Event.ReasonPhrase = status.ToName();
			this.Body.Event.Description = description;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="result"></param>
		internal void Fill(QueryResult result) {
			this.Verb = result.Verb;
			this.credential = ((IMessageDto)result).Credential;
			this.body = ((IMessageDto)result).Body;
			this.IsClosed = result.IsClosed;
			this.IsDumb = result.IsDumb;
			this.Ontology = result.Ontology;
			this.MessageID = result.MessageID;
			this.StackTrace = result.StackTrace;
			this.TimeStamp = result.TimeStamp;
			this.MessageType = result.MessageType;
			this.Version = result.Version;
			this.From = result.From;
			this.SessionID = result.SessionID;
			this.To = result.To;
			this.RelatesTo = result.RelatesTo;
		}
	}
}

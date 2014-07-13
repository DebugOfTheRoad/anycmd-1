
namespace Anycmd.Host.EDI.Hecp {
	using System;

	/// <summary>A helper class for retrieving and comparing standard Hecp actionCodes.</summary>
	public class Verb : IEquatable<Verb> {
		private string actionCode;
		private static readonly Verb getAction = new Verb("GET");
		private static readonly Verb updateAction = new Verb("UPDATE");
		private static readonly Verb createAction = new Verb("CREATE");
		private static readonly Verb deleteAction = new Verb("DELETE");
		private static readonly Verb headAction = new Verb("HEAD");

		/// <summary>
		/// Represents an Hecp GET protocol actionCode.
		/// </summary>
		/// <returns>
		/// Returns <see cref="Verb" />.
		/// </returns>
		public static Verb Get {
			get {
				return Verb.getAction;
			}
		}

		/// <summary>
		/// Represents an Hecp PUT protocol actionCode that is used to replace an entity identified by a URI.
		/// </summary>
		/// <returns>
		/// Returns <see cref="Verb" />.
		/// </returns>
		public static Verb Update {
			get {
				return Verb.updateAction;
			}
		}

		/// <summary>
		/// Represents an Hecp POST protocol actionCode that is used to post a new entity as an addition to a URI.
		/// </summary>
		/// <returns>
		/// Returns <see cref="Verb" />.
		/// </returns>
		public static Verb Create {
			get {
				return Verb.createAction;
			}
		}

		/// <summary>
		/// Represents an Hecp DELETE protocol actionCode.
		/// </summary>
		/// <returns>
		/// Returns <see cref="Verb" />.
		/// </returns>
		public static Verb Delete {
			get {
				return Verb.deleteAction;
			}
		}

		/// <summary>
		/// Represents an Hecp HEAD protocol actionCode. The HEAD actionCode is identical to GET except that the server only returns message-headers in the response, without a message-body.
		/// </summary>
		/// <returns>
		/// Returns <see cref="Verb" />.
		/// </returns>
		public static Verb Head {
			get {
				return Verb.headAction;
			}
		}

		/// <summary>
		/// An Hecp actionCode. 
		/// </summary>
		/// <returns>
		/// Returns <see cref="T:System.String" />.An Hecp actionCode represented as a <see cref="T:System.String" />.
		/// </returns>
		public string Code {
			get {
				return this.actionCode;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Verb" /> class with a specific Hecp actionCode.
		/// </summary>
		/// <param name="actionCode">The Hecp actionCode.</param>
		public Verb(string actionCode) {
			this.actionCode = actionCode ?? string.Empty;
		}

		/// <returns>
		/// Returns <see cref="T:System.Boolean" />.
		/// </returns>
		public bool Equals(Verb other) {
			return other != null && (ReferenceEquals(this.actionCode, other.actionCode) || String.Compare(this.actionCode, other.actionCode, StringComparison.OrdinalIgnoreCase) == 0);
		}

		/// <returns>
		/// Returns <see cref="T:System.Boolean" />.
		/// </returns>
		public override bool Equals(object obj) {
			return this.Equals(obj as Verb);
		}

		/// <returns>
		/// Returns <see cref="T:System.Int32" />.
		/// </returns>
		public override int GetHashCode() {
			return this.Code.ToUpperInvariant().GetHashCode();
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// Returns <see cref="T:System.String" />.A string representing the current object.
		/// </returns>
		public override string ToString() {
			return this.actionCode.ToString();
		}

		/// <returns>
		/// Returns <see cref="T:System.Boolean" />.
		/// </returns>
		public static bool operator ==(Verb left, Verb right) {
			if (ReferenceEquals(left, null)) {
				return ReferenceEquals(right, null);
			}
			if (ReferenceEquals(right, null)) {
				return ReferenceEquals(left, null);
			}
			return ReferenceEquals(left, right) || left.Code.Equals(right.Code, StringComparison.OrdinalIgnoreCase);
		}

		/// <returns>
		/// Returns <see cref="T:System.Boolean" />.
		/// </returns>
		public static bool operator !=(Verb left, Verb right) {
			return !(left == right);
		}
	}
}

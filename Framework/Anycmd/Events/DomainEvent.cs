using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Anycmd.Events
{
    using Model;
    using Util;

    /// <summary>
    /// Represents the base class for all domain events.
    /// </summary>
    [Serializable]
    public abstract class DomainEvent : IDomainEvent
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>DomainEvent</c> class.
        /// </summary>
        public DomainEvent()
        {
            this.Timestamp = DateTime.Now;
        }
        /// <summary>
        /// Initializes a new instace of <c>DomainEvent</c> class.
        /// </summary>
        /// <param name="source">The source entity which raises the domain event.</param>
        public DomainEvent(IEntity source)
            : this()
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            this.Source = source;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns the hash code for current domain event.
        /// </summary>
        /// <returns>The calculated hash code for the current domain event.</returns>
        public override int GetHashCode()
        {
            return ReflectionHelper.GetHashCode(this.Source.GetHashCode(),
                this.Branch.GetHashCode(),
                this.Id.GetHashCode(),
                this.Timestamp.GetHashCode(),
                this.Version.GetHashCode());
        }
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value indicating whether this instance is equal to a specified
        /// entity.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>True if obj is an instance of the <see cref="Anycmd.ISourcedAggregateRoot"/> type and equals the value of this
        /// instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj == null)
                return false;
            DomainEvent other = obj as DomainEvent;
            if ((object)other == (object)null)
                return false;
            return this.Id == other.Id;
        }
        #endregion

        #region IDomainEvent Members
        /// <summary>
        /// Gets or sets the source entity from which the domain event was generated.
        /// </summary>
        [XmlIgnore]
        [SoapIgnore]
        [IgnoreDataMember]
        public IEntity Source
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the version of the domain event.
        /// </summary>
        public virtual long Version { get; set; }
        /// <summary>
        /// Gets or sets the branch on which the current version of domain event exists.
        /// </summary>
        public virtual long Branch { get; set; }
        /// <summary>
        /// Gets or sets the assembly qualified type name of the event.
        /// </summary>
        public virtual string AssemblyQualifiedEventType { get; set; }
        #endregion

        #region IEvent Members
        /// <summary>
        /// Gets or sets the date and time on which the event was produced.
        /// </summary>
        /// <remarks>The format of this date/time value could be various between different
        /// systems. Apworks recommend system designer or architect uses the standard
        /// UTC date/time format.</remarks>
        public virtual DateTime Timestamp { get; set; }
        #endregion

        #region IEntity Members
        /// <summary>
        /// Gets or sets the identifier of the domain event.
        /// </summary>
        public virtual Guid Id { get; set; }
        #endregion
    }
}

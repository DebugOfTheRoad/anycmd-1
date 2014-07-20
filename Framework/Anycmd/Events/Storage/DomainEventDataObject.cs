﻿
namespace Anycmd.Events.Storage
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the domain event data object which holds the data of a specific domain event.
    /// </summary>
    /// <remarks>The <c>DomainEventDataObject</c> class implemented the Data Transfer Object pattern
    /// that was described in Martin Fowler's book, Patterns of Enterprise Application Architecture.
    /// For more information about Data Transfer Object pattern, please refer to http://martinfowler.com/eaaCatalog/dataTransferObject.html.
    /// </remarks>
    [Serializable]
    [XmlRoot]
    [DataContract]
    public class DomainEventDataObject
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the domain event data object.
        /// </summary>
        public DomainEventDataObject()
        {
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets or sets an array of <see cref="System.Byte"/> value which holds the data
        /// of current domain event object.
        /// </summary>
        [XmlElement]
        [DataMember]
        public byte[] Data { get; set; }
        /// <summary>
        /// Gets or sets the assembly qualified name of the type of the domain event.
        /// </summary>
        [XmlElement]
        [DataMember]
        public string AssemblyQualifiedEventType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the branch on which domain event data object exists.
        /// </summary>
        [XmlElement]
        [DataMember]
        public long Branch
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the identifier of the domain event.
        /// </summary>
        /// <remarks>Note that since the <c>DomainEventDataObject</c> is the data
        /// presentation of the corresponding domain event object, this identifier value
        /// can also be considered to be the identifier for the <c>DomainEventDataObject</c> instance.</remarks>
        [XmlElement]
        [DataMember]
        public Guid Id
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the identifier of the aggregate root which holds the instance
        /// of the current domain event.
        /// </summary>
        [XmlElement]
        [DataMember]
        public Guid SourceID
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the assembly qualified name of the type of the aggregate root.
        /// </summary>
        [XmlElement]
        [DataMember]
        public string AssemblyQualifiedSourceType
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the date and time on which the event was produced.
        /// </summary>
        /// <remarks>The format of this date/time value could be various between different
        /// systems. Apworks recommend system designer or architect uses the standard
        /// UTC date/time format.</remarks>
        [XmlElement]
        [DataMember]
        public DateTime Timestamp
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the version of the domain event data object.
        /// </summary>
        [XmlElement]
        [DataMember]
        public long Version
        {
            get;
            set;
        }
        #endregion
    }
}

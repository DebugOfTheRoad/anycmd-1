﻿
using Anycmd.Serialization;

namespace Anycmd.Events.Serialization
{
    /// <summary>
    /// Represents the serializer for domain events that serializes/deserializes the domain events
    /// with DataContract format.
    /// </summary>
    public class DomainEventDataContractSerializer : ObjectDataContractSerializer, IDomainEventSerializer
    {
    }
}

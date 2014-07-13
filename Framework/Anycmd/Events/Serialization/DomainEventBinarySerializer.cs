﻿
using Anycmd.Serialization;

namespace Anycmd.Events.Serialization
{
    /// <summary>
    /// Represents the serializer for domain events that serializes/deserializes the domain events
    /// with binary format.
    /// </summary>
    public class DomainEventBinarySerializer : ObjectBinarySerializer, IDomainEventSerializer
    {

    }
}

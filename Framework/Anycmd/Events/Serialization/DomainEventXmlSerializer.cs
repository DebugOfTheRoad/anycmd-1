
using Anycmd.Serialization;

namespace Anycmd.Events.Serialization
{
    /// <summary>
    /// Represents the serializer for domain events that serializes/deserializes the domain events
    /// with XML format.
    /// </summary>
    public class DomainEventXmlSerializer : ObjectXmlSerializer, IDomainEventSerializer
    {
    }
}


namespace Anycmd.Serialization
{
    /// <summary>
    /// Represents that the implemented classes are object serializers.
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// Serializes an object into a byte stream.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="obj">The object to be serialized.</param>
        /// <returns>The byte stream which contains the serialized data.</returns>
        byte[] Serialize<TObject>(TObject obj);
        /// <summary>
        /// Deserializes an object from the given byte stream.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="stream">The byte stream which contains the serialized data of the object.</param>
        /// <returns>The deserialized object.</returns>
        TObject Deserialize<TObject>(byte[] stream);
    }
}

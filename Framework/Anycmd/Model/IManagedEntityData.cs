
using Anycmd.Host;
namespace Anycmd.Model
{

    public interface IManagedEntityData
    {
        EntityTypeState EntityType { get; }
        IManagedPropertyValues InputValues { get; }
        IManagedPropertyValues Entity { get; }
    }
}

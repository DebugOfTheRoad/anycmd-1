using Anycmd.Host;
using System.Collections.Generic;

namespace Anycmd.Model
{
    public interface IManagedPropertyValues
    {
        IEnumerable<ManagedPropertyValue> GetValues(EntityTypeState entityType);
    }
}

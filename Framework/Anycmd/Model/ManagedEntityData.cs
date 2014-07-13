
namespace Anycmd.Model
{
    using Anycmd.Host;
    using System;

    public class ManagedEntityData : IManagedEntityData
    {
        public ManagedEntityData(EntityTypeState entityType, IManagedPropertyValues entity, IManagedPropertyValues inputValues)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            this.EntityType = entityType;
            this.Entity = entity;
            this.InputValues = inputValues;
        }

        public EntityTypeState EntityType { get; private set; }

        public IManagedPropertyValues Entity { get; private set; }

        public IManagedPropertyValues InputValues { get; private set; }
    }
}

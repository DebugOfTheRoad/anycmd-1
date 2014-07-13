
namespace Anycmd.Model
{
    using Anycmd.Host;

    public class ManagedEntityData<TEntity, TInputModel> : ManagedEntityData
        where TEntity : IManagedPropertyValues
        where TInputModel : IManagedPropertyValues
    {
        public ManagedEntityData(EntityTypeState entityType, TEntity entity, TInputModel inputValue)
            : base(entityType, entity, inputValue)
        {
            this.Entity = entity;
            this.InputValues = inputValue;
        }

        public new TEntity Entity { get; private set; }

        public new TInputModel InputValues { get; private set; }
    }
}

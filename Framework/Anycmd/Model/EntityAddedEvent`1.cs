using Anycmd.Events;
using System;

namespace Anycmd.Model
{
    public class EntityAddedEvent<TEntityCreateInput> : DomainEvent where TEntityCreateInput : IEntityCreateInput
    {
        public EntityAddedEvent(IEntity source, TEntityCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public TEntityCreateInput Input { get; private set; }
    }
}

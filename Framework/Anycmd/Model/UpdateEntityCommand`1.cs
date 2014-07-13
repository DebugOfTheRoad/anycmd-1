
namespace Anycmd.Model
{
    using Commands;
    using System;

    public abstract class UpdateEntityCommand<TEntityUpdateInput> : Command where TEntityUpdateInput : IEntityUpdateInput
    {
        public UpdateEntityCommand(TEntityUpdateInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public TEntityUpdateInput Input { get; private set; }
    }
}

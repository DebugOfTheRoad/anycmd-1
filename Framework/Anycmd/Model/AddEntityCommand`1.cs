
namespace Anycmd.Model
{
    using Commands;
    using System;

    public abstract class AddEntityCommand<TEntityCreateInput> : Command where TEntityCreateInput : IEntityCreateInput
    {
        public AddEntityCommand(TEntityCreateInput input)
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

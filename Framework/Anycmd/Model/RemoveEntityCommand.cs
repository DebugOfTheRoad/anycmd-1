
namespace Anycmd.Model
{
    using Commands;
    using System;

    public abstract class RemoveEntityCommand: Command
    {
        public RemoveEntityCommand(Guid entityID)
        {
            this.EntityID = entityID;
        }

        public Guid EntityID { get; private set; }
    }
}

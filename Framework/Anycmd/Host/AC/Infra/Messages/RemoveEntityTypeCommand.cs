
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveEntityTypeCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveEntityTypeCommand(Guid entityTypeID)
            : base(entityTypeID)
        {

        }
    }
}

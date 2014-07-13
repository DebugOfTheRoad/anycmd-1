
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveResourceTypeCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveResourceTypeCommand(Guid resourceTypeID)
            : base(resourceTypeID)
        {

        }
    }
}

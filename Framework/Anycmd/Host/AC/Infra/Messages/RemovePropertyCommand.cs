
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;


    public class RemovePropertyCommand : RemoveEntityCommand, ISysCommand
    {
        public RemovePropertyCommand(Guid propertyID)
            : base(propertyID)
        {

        }
    }
}

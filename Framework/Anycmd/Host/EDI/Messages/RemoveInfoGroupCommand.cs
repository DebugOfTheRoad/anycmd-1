
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveInfoGroupCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveInfoGroupCommand(Guid infoGroupID)
            : base(infoGroupID)
        {

        }
    }
}

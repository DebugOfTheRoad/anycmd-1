
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveActionCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveActionCommand(Guid actionID)
            : base(actionID)
        {

        }
    }
}

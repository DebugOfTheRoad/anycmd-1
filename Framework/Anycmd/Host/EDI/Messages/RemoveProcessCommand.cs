
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveProcessCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveProcessCommand(Guid processID)
            : base(processID)
        {

        }
    }
}

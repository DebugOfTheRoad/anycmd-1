
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveInfoDicCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveInfoDicCommand(Guid infoDicID)
            : base(infoDicID)
        {

        }
    }
}

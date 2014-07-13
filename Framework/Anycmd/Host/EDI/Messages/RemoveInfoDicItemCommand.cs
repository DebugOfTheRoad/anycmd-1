
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveInfoDicItemCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveInfoDicItemCommand(Guid infoDicItemID)
            : base(infoDicItemID)
        {

        }
    }
}

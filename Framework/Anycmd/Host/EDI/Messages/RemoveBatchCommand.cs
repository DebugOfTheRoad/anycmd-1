
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveBatchCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveBatchCommand(Guid batchID)
            : base(batchID)
        {

        }
    }
}

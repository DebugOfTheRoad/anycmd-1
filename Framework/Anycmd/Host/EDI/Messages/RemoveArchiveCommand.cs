
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveArchiveCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveArchiveCommand(Guid archiveID)
            : base(archiveID)
        {

        }
    }
}

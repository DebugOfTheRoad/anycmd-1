
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveNodeElementCareCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveNodeElementCareCommand(Guid nodeElementCareID)
            : base(nodeElementCareID)
        {

        }
    }
}

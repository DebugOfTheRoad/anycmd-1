
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveNodeCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveNodeCommand(Guid nodeID)
            : base(nodeID)
        {

        }
    }
}

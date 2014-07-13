
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveNodeElementActionCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveNodeElementActionCommand(Guid nodeElementActionID)
            : base(nodeElementActionID)
        {

        }
    }
}

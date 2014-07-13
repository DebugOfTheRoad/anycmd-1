
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveNodeOntologyCareCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveNodeOntologyCareCommand(Guid nodeOntologyCareID)
            : base(nodeOntologyCareID)
        {

        }
    }
}

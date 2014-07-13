
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveOntologyCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveOntologyCommand(Guid ontologyID)
            : base(ontologyID)
        {

        }
    }
}

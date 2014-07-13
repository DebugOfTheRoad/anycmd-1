
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateOntologyCommand: UpdateEntityCommand<IOntologyUpdateInput>, ISysCommand
    {
        public UpdateOntologyCommand(IOntologyUpdateInput input)
            : base(input)
        {

        }
    }
}

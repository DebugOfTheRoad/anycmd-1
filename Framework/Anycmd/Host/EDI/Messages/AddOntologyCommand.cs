
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddOntologyCommand : AddEntityCommand<IOntologyCreateInput>, ISysCommand
    {
        public AddOntologyCommand(IOntologyCreateInput input)
            : base(input)
        {

        }
    }
}

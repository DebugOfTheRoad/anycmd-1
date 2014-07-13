
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddNodeOntologyCareCommand: AddEntityCommand<INodeOntologyCareCreateInput>, ISysCommand
    {
        public AddNodeOntologyCareCommand(INodeOntologyCareCreateInput input)
            : base(input)
        {

        }
    }
}

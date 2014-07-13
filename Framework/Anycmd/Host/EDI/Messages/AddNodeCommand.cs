
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddNodeCommand : AddEntityCommand<INodeCreateInput>, ISysCommand
    {
        public AddNodeCommand(INodeCreateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddNodeElementActionCommand : AddEntityCommand<INodeElementActionCreateInput>, ISysCommand
    {
        public AddNodeElementActionCommand(INodeElementActionCreateInput input)
            : base(input)
        {

        }
    }
}

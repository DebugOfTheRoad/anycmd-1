
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddNodeElementCareCommand : AddEntityCommand<INodeElementCareCreateInput>, ISysCommand
    {
        public AddNodeElementCareCommand(INodeElementCareCreateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateNodeCommand : UpdateEntityCommand<INodeUpdateInput>, ISysCommand
    {
        public UpdateNodeCommand(INodeUpdateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddInfoGroupCommand : AddEntityCommand<IInfoGroupCreateInput>, ISysCommand
    {
        public AddInfoGroupCommand(IInfoGroupCreateInput input)
            : base(input)
        {

        }
    }
}

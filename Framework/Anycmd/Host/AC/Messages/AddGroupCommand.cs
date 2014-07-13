
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddGroupCommand : AddEntityCommand<IGroupCreateInput>, ISysCommand
    {
        public AddGroupCommand(IGroupCreateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateGroupCommand : UpdateEntityCommand<IGroupUpdateInput>, ISysCommand
    {
        public UpdateGroupCommand(IGroupUpdateInput input)
            : base(input)
        {

        }
    }
}

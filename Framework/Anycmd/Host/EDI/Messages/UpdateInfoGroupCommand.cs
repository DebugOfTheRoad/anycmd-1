
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateInfoGroupCommand : UpdateEntityCommand<IInfoGroupUpdateInput>, ISysCommand
    {
        public UpdateInfoGroupCommand(IInfoGroupUpdateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateActionCommand : UpdateEntityCommand<IActionUpdateInput>, ISysCommand
    {
        public UpdateActionCommand(IActionUpdateInput input)
            : base(input)
        {

        }
    }
}

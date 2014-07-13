
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateProcessCommand : UpdateEntityCommand<IProcessUpdateInput>, ISysCommand
    {
        public UpdateProcessCommand(IProcessUpdateInput input)
            : base(input)
        {

        }
    }
}

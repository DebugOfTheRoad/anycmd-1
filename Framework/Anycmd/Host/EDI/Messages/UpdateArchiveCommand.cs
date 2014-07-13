
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateArchiveCommand : UpdateEntityCommand<IArchiveUpdateInput>, ISysCommand
    {
        public UpdateArchiveCommand(IArchiveUpdateInput input)
            : base(input)
        {

        }
    }
}

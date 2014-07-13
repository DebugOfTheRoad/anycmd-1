
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddArchiveCommand : AddEntityCommand<IArchiveCreateInput>, ISysCommand
    {
        public AddArchiveCommand(IArchiveCreateInput input)
            : base(input)
        {

        }
    }
}

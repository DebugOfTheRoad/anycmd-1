
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddProcessCommand : AddEntityCommand<IProcessCreateInput>, ISysCommand
    {
        public AddProcessCommand(IProcessCreateInput input)
            : base(input)
        {

        }
    }
}

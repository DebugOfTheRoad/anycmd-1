
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddActionCommand : AddEntityCommand<IActionCreateInput>, ISysCommand
    {
        public AddActionCommand(IActionCreateInput input)
            : base(input)
        {

        }
    }
}

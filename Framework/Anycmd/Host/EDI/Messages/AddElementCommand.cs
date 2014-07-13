
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddElementCommand : AddEntityCommand<IElementCreateInput>, ISysCommand
    {
        public AddElementCommand(IElementCreateInput input)
            : base(input)
        {

        }
    }
}

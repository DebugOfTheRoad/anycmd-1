
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddInfoDicCommand : AddEntityCommand<IInfoDicCreateInput>, ISysCommand
    {
        public AddInfoDicCommand(IInfoDicCreateInput input)
            : base(input)
        {

        }
    }
}

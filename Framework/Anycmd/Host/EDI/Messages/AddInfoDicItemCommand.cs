
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddInfoDicItemCommand : AddEntityCommand<IInfoDicItemCreateInput>, ISysCommand
    {
        public AddInfoDicItemCommand(IInfoDicItemCreateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddTopicCommand : AddEntityCommand<ITopicCreateInput>, ISysCommand
    {
        public AddTopicCommand(ITopicCreateInput input)
            : base(input)
        {

        }
    }
}

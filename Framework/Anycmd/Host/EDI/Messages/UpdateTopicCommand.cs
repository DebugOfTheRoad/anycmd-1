
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateTopicCommand : UpdateEntityCommand<ITopicUpdateInput>, ISysCommand
    {
        public UpdateTopicCommand(ITopicUpdateInput input)
            : base(input)
        {

        }
    }
}

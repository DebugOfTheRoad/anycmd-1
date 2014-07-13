
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddBatchCommand : AddEntityCommand<IBatchCreateInput>, ISysCommand
    {
        public AddBatchCommand(IBatchCreateInput input)
            : base(input)
        {

        }
    }
}

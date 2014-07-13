
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateBatchCommand : UpdateEntityCommand<IBatchUpdateInput>, ISysCommand
    {
        public UpdateBatchCommand(IBatchUpdateInput input)
            : base(input)
        {

        }
    }
}

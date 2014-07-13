
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateInfoDicItemCommand : UpdateEntityCommand<IInfoDicItemUpdateInput>, ISysCommand
    {
        public UpdateInfoDicItemCommand(IInfoDicItemUpdateInput input)
            : base(input)
        {

        }
    }
}

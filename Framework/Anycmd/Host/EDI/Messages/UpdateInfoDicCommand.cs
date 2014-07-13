
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateInfoDicCommand : UpdateEntityCommand<IInfoDicUpdateInput>, ISysCommand
    {
        public UpdateInfoDicCommand(IInfoDicUpdateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateDicItemCommand : UpdateEntityCommand<IDicItemUpdateInput>, ISysCommand
    {
        public UpdateDicItemCommand(IDicItemUpdateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class UpdateDicCommand : UpdateEntityCommand<IDicUpdateInput>, ISysCommand
    {
        public UpdateDicCommand(IDicUpdateInput input)
            : base(input)
        {

        }
    }
}

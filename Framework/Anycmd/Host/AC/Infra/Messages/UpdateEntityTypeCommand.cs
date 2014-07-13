
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class UpdateEntityTypeCommand : UpdateEntityCommand<IEntityTypeUpdateInput>, ISysCommand
    {
        public UpdateEntityTypeCommand(IEntityTypeUpdateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class UpdateButtonCommand : UpdateEntityCommand<IButtonUpdateInput>, ISysCommand
    {
        public UpdateButtonCommand(IButtonUpdateInput input)
            : base(input)
        {

        }
    }
}

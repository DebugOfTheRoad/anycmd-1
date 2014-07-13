
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdatePropertyCommand : UpdateEntityCommand<IPropertyUpdateInput>, ISysCommand
    {
        public UpdatePropertyCommand(IPropertyUpdateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateMenuCommand : UpdateEntityCommand<IMenuUpdateInput>, ISysCommand
    {
        public UpdateMenuCommand(IMenuUpdateInput input)
            : base(input)
        {

        }
    }
}

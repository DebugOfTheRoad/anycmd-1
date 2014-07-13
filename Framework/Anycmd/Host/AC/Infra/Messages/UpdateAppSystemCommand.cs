
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateAppSystemCommand : UpdateEntityCommand<IAppSystemUpdateInput>, ISysCommand
    {
        public UpdateAppSystemCommand(IAppSystemUpdateInput input)
            : base(input)
        {

        }
    }
}

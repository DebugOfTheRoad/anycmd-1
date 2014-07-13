
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddAppSystemCommand : AddEntityCommand<IAppSystemCreateInput>, ISysCommand
    {
        public AddAppSystemCommand(IAppSystemCreateInput input)
            : base(input)
        {

        }
    }
}

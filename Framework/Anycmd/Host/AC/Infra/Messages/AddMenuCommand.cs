
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddMenuCommand : AddEntityCommand<IMenuCreateInput>, ISysCommand
    {
        public AddMenuCommand(IMenuCreateInput input)
            : base(input)
        {

        }
    }
}

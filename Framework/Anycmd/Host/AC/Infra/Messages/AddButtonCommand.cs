
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddButtonCommand : AddEntityCommand<IButtonCreateInput>, ISysCommand
    {
        public AddButtonCommand(IButtonCreateInput input)
            : base(input)
        {

        }
    }
}

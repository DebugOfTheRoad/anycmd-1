
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddEntityTypeCommand : AddEntityCommand<IEntityTypeCreateInput>, ISysCommand
    {
        public AddEntityTypeCommand(IEntityTypeCreateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddFunctionCommand : AddEntityCommand<IFunctionCreateInput>, ISysCommand
    {
        public AddFunctionCommand(IFunctionCreateInput input)
            : base(input)
        {

        }
    }
}

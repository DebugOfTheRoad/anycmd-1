
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class UpdateFunctionCommand : UpdateEntityCommand<IFunctionUpdateInput>, ISysCommand
    {
        public UpdateFunctionCommand(IFunctionUpdateInput input)
            : base(input)
        {

        }
    }
}

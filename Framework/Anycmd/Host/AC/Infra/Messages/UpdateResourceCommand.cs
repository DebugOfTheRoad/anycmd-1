
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateResourceCommand : UpdateEntityCommand<IResourceUpdateInput>, ISysCommand
    {
        public UpdateResourceCommand(IResourceUpdateInput input)
            : base(input)
        {

        }
    }
}

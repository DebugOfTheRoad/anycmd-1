
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddResourceCommand : AddEntityCommand<IResourceCreateInput>, ISysCommand
    {
        public AddResourceCommand(IResourceCreateInput input)
            : base(input)
        {

        }
    }
}

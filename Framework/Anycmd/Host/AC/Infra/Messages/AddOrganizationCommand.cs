
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddOrganizationCommand : AddEntityCommand<IOrganizationCreateInput>, ISysCommand
    {
        public AddOrganizationCommand(IOrganizationCreateInput input)
            : base(input)
        {

        }
    }
}

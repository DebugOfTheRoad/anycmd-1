
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveOrganizationCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveOrganizationCommand(Guid organizationID)
            : base(organizationID)
        {

        }
    }
}

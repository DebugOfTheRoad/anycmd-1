using Anycmd.Commands;
using System;

namespace Anycmd.Host.EDI.Messages
{
    public class ChangeProcessOrganizationCommand : Command, ISysCommand
    {
        public ChangeProcessOrganizationCommand(Guid processID, string organizationCode)
        {
            this.ProcessID = processID;
            this.OrganizationCode = organizationCode;
        }

        public Guid ProcessID { get; private set; }
        public string OrganizationCode { get; private set; }
    }
}

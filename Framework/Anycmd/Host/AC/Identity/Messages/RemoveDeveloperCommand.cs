
namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;
    using System;

    public class RemoveDeveloperCommand: Command, ISysCommand
    {
        public RemoveDeveloperCommand(Guid accountID)
        {
            this.AccountID = accountID;
        }

        public Guid AccountID { get; private set; }
    }
}

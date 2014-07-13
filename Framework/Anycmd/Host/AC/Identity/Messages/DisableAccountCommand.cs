using Anycmd.Commands;
using System;

namespace Anycmd.Host.AC.Identity.Messages
{
    public class DisableAccountCommand : Command, ISysCommand
    {
        public DisableAccountCommand(Guid accountID)
        {
            this.AccountID = accountID;
        }

        public Guid AccountID { get; private set; }
    }
}

using Anycmd.Commands;
using System;

namespace Anycmd.Host.AC.Identity.Messages
{
    public class EnableAccountCommand : Command
    {
        public EnableAccountCommand(Guid accountID)
        {
            this.AccountID = accountID;
        }

        public Guid AccountID { get; private set; }
    }
}

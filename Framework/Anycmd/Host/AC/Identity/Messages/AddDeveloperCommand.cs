using Anycmd.Commands;
using System;

namespace Anycmd.Host.AC.Identity.Messages
{
    public class AddDeveloperCommand : Command, ISysCommand
    {
        public AddDeveloperCommand(Guid accountID)
        {
            this.AccountID = accountID;
        }

        public Guid AccountID { get; private set; }
    }
}

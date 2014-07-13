
namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveAccountCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveAccountCommand(Guid accountID)
            : base(accountID)
        {

        }
    }
}

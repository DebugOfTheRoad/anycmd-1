
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveRoleCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveRoleCommand(Guid roleID)
            : base(roleID)
        {

        }
    }
}

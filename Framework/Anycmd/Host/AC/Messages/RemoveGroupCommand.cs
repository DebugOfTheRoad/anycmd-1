
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveGroupCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveGroupCommand(Guid groupID)
            : base(groupID)
        {

        }
    }
}

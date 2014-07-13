
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveDicItemCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveDicItemCommand(Guid dicItemID)
            : base(dicItemID)
        {

        }
    }
}

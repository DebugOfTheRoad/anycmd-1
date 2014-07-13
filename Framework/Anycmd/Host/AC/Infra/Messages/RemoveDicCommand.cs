
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveDicCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveDicCommand(Guid dicID)
            : base(dicID)
        {

        }
    }
}

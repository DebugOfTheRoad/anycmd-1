
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveButtonCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveButtonCommand(Guid buttonID)
            : base(buttonID)
        {

        }
    }
}


namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveAppSystemCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveAppSystemCommand(Guid appSystemID)
            : base(appSystemID)
        {

        }
    }
}

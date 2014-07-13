
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveMenuCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveMenuCommand(Guid menuID)
            : base(menuID)
        {

        }
    }
}

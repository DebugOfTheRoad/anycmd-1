
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemovePageCommand : RemoveEntityCommand, ISysCommand
    {
        public RemovePageCommand(Guid pageID)
            : base(pageID)
        {

        }
    }
}

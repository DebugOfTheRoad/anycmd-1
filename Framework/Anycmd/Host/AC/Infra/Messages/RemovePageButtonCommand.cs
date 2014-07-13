
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemovePageButtonCommand : RemoveEntityCommand, ISysCommand
    {
        public RemovePageButtonCommand(Guid pageButtonID)
            : base(pageButtonID)
        {

        }
    }
}

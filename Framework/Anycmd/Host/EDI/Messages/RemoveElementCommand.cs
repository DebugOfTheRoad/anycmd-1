
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveElementCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveElementCommand(Guid elementID)
            : base(elementID)
        {

        }
    }
}

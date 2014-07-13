
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemoveTopicCommand : RemoveEntityCommand, ISysCommand
    {
        public RemoveTopicCommand(Guid eventTopicID)
            : base(eventTopicID)
        {

        }
    }
}

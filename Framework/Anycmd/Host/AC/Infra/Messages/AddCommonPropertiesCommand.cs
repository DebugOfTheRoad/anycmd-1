using Anycmd.Commands;
using System;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class AddCommonPropertiesCommand : Command, ISysCommand
    {
        public AddCommonPropertiesCommand(Guid entityTypeID)
        {
            this.EntityTypeID = entityTypeID;
        }

        public Guid EntityTypeID { get; private set; }
    }
}

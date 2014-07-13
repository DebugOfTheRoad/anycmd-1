using Anycmd.Commands;
using System;

namespace Anycmd.Host.EDI.Messages
{
    public class ChangeProcessNetPortCommand: Command, ISysCommand
    {
        public ChangeProcessNetPortCommand(Guid processID, int netPort)
        {
            this.ProcessID = processID;
            this.NetPort = netPort;
        }

        public Guid ProcessID { get; private set; }
        public int NetPort { get; private set; }
    }
}

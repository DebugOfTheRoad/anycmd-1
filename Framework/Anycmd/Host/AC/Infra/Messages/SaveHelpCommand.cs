using Anycmd.Commands;
using System;

namespace Anycmd.Host.AC.Infra.Messages
{
    public class SaveHelpCommand : Command, ISysCommand
    {
        public SaveHelpCommand(Guid functionID, string content, int? isEnabled)
        {
            this.FunctionID = functionID;
            this.Content = content;
            this.IsEnabled = IsEnabled;
        }

        public Guid FunctionID { get; private set; }
        public string Content { get; private set; }
        public int? IsEnabled { get; private set; }
    }
}

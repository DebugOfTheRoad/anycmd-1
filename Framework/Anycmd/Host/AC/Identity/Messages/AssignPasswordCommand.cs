
namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;
    using System;
    using ValueObjects;

    public class AssignPasswordCommand : Command, ISysCommand
    {
        public AssignPasswordCommand(IPasswordAssignInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public IPasswordAssignInput Input { get; private set; }
    }
}


namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;
    using System;
    using ValueObjects;

    public class ChangePasswordCommand : Command, ISysCommand
    {
        public ChangePasswordCommand(IPasswordChangeInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public IPasswordChangeInput Input { get; private set; }
    }
}

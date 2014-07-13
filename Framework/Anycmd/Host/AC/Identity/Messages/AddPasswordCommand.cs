
namespace Anycmd.Host.AC.Identity.Messages
{
    using Commands;
    using System;
    using ValueObjects;

    public class AddPasswordCommand : Command, ISysCommand
    {
        public AddPasswordCommand(IPasswordCreateInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            this.Input = input;
        }

        public IPasswordCreateInput Input { get; private set; }
    }
}

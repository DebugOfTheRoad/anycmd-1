
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using System;

    public class RemovePrivilegeBigramCommand : RemoveEntityCommand, ISysCommand
    {
        public RemovePrivilegeBigramCommand(Guid privilegeBigramID)
            : base(privilegeBigramID)
        {

        }
    }
}

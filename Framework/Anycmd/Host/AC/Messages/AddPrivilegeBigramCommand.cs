
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class AddPrivilegeBigramCommand : AddEntityCommand<IPrivilegeBigramCreateInput>, ISysCommand
    {
        public AddPrivilegeBigramCommand(IPrivilegeBigramCreateInput input)
            : base(input)
        {

        }
    }
}

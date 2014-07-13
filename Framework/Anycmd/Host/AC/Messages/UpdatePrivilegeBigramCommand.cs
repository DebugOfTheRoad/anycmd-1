
namespace Anycmd.Host.AC.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdatePrivilegeBigramCommand : UpdateEntityCommand<IPrivilegeBigramUpdateInput>, ISysCommand
    {
        public UpdatePrivilegeBigramCommand(IPrivilegeBigramUpdateInput input)
            : base(input)
        {

        }
    }
}

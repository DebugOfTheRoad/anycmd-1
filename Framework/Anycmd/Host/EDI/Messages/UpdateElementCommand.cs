
namespace Anycmd.Host.EDI.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdateElementCommand : UpdateEntityCommand<IElementUpdateInput>, ISysCommand
    {
        public UpdateElementCommand(IElementUpdateInput input)
            : base(input)
        {

        }
    }
}

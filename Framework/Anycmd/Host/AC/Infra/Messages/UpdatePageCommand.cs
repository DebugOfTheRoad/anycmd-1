
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;

    public class UpdatePageCommand : UpdateEntityCommand<IPageUpdateInput>, ISysCommand
    {
        public UpdatePageCommand(IPageUpdateInput input)
            : base(input)
        {

        }
    }
}

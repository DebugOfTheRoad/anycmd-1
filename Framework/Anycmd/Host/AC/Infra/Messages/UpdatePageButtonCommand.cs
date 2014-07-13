
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class UpdatePageButtonCommand : UpdateEntityCommand<IPageButtonUpdateInput>, ISysCommand
    {
        public UpdatePageButtonCommand(IPageButtonUpdateInput input)
            : base(input)
        {

        }
    }
}

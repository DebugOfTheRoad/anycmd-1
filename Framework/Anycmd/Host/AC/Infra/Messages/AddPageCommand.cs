
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddPageCommand : AddEntityCommand<IPageCreateInput>, ISysCommand
    {
        public AddPageCommand(IPageCreateInput input)
            : base(input)
        {

        }
    }
}

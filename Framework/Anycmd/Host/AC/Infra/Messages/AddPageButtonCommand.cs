
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddPageButtonCommand : AddEntityCommand<IPageButtonCreateInput>, ISysCommand
    {
        public AddPageButtonCommand(IPageButtonCreateInput input)
            : base(input)
        {

        }
    }
}

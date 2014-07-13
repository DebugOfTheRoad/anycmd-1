
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddDicItemCommand : AddEntityCommand<IDicItemCreateInput>, ISysCommand
    {
        public AddDicItemCommand(IDicItemCreateInput input)
            : base(input)
        {

        }
    }
}


namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddDicCommand : AddEntityCommand<IDicCreateInput>, ISysCommand
    {
        public AddDicCommand(IDicCreateInput input)
            : base(input)
        {

        }
    }
}

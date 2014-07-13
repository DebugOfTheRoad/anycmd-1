
namespace Anycmd.Host.AC.Infra.Messages
{
    using Commands;
    using Model;
    using ValueObjects;


    public class AddPropertyCommand : AddEntityCommand<IPropertyCreateInput>, ISysCommand
    {
        public AddPropertyCommand(IPropertyCreateInput input)
            : base(input)
        {

        }
    }
}

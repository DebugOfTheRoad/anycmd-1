
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class ButtonAddedEvent : EntityAddedEvent<IButtonCreateInput>
    {
        public ButtonAddedEvent(ButtonBase source, IButtonCreateInput input)
            : base(source, input)
        {
        }
    }
}
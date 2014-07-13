
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    public class MenuAddedEvent : EntityAddedEvent<IMenuCreateInput>
    {
        #region Ctor
        public MenuAddedEvent(MenuBase source, IMenuCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}
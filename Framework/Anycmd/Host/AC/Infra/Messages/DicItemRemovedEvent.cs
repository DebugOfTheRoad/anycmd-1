
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class DicItemRemovedEvent : DomainEvent
    {
        #region Ctor
        public DicItemRemovedEvent(DicItemBase source)
            : base(source)
        {
        }
        #endregion
    }
}
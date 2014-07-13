
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class InfoDicItemRemovedEvent : DomainEvent {
        #region Ctor
        public InfoDicItemRemovedEvent(InfoDicItemBase source) : base(source) { }
        #endregion
    }
}

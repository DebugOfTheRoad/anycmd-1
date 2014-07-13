
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class InfoDicRemovedEvent : DomainEvent {
        #region Ctor
        public InfoDicRemovedEvent(InfoDicBase source) : base(source) { }
        #endregion
    }
}

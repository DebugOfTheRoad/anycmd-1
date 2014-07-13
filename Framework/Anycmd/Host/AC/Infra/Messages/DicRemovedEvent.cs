
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class DicRemovedEvent : DomainEvent
    {
        #region Ctor
        public DicRemovedEvent(DicBase source)
            : base(source)
        {
        }
        #endregion
    }
}
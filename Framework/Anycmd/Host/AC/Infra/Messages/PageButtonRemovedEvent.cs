
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class PageButtonRemovedEvent : DomainEvent
    {
        #region Ctor
        public PageButtonRemovedEvent(PageButtonBase source)
            : base(source)
        {
        }
        #endregion
    }
}

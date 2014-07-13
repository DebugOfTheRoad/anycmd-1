
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class PageRemovedEvent : DomainEvent
    {
        #region Ctor
        public PageRemovedEvent(PageBase source)
            : base(source)
        {
        }
        #endregion
    }
}

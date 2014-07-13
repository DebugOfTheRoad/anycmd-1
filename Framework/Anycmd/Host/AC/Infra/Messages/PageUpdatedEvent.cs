
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class PageUpdatedEvent : DomainEvent
    {
        #region Ctor
        public PageUpdatedEvent(PageBase source, IPageUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IPageUpdateInput Input { get; private set; }
    }
}

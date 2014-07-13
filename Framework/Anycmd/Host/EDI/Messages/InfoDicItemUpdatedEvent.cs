
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class InfoDicItemUpdatedEvent : DomainEvent
    {
        #region Ctor
        public InfoDicItemUpdatedEvent(InfoDicItemBase source, IInfoDicItemUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IInfoDicItemUpdateInput Input { get; private set; }
    }
}

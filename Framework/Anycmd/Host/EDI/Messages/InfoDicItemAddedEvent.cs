
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class InfoDicItemAddedEvent : DomainEvent
    {
        #region Ctor
        public InfoDicItemAddedEvent(InfoDicItemBase source, IInfoDicItemCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IInfoDicItemCreateInput Input { get; private set; }
    }
}

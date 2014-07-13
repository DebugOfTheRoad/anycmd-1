
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class InfoDicUpdatedEvent : DomainEvent
    {
        #region Ctor
        public InfoDicUpdatedEvent(InfoDicBase source, IInfoDicUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IInfoDicUpdateInput Input { get; private set; }
    }
}

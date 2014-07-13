
namespace Anycmd.Host.EDI.Messages
{
    using Anycmd.EDI;
    using Anycmd.Events;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class InfoDicAddedEvent : DomainEvent
    {
        #region Ctor
        public InfoDicAddedEvent(InfoDicBase source, IInfoDicCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IInfoDicCreateInput Input { get; private set; }
    }
}

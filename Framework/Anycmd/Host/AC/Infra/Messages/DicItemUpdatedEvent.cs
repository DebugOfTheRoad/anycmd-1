
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class DicItemUpdatedEvent : DomainEvent
    {
        #region Ctor
        public DicItemUpdatedEvent(DicItemBase source, IDicItemUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("inpu");
            }
            this.Input = input;
        }
        #endregion

        public IDicItemUpdateInput Input { get; private set; }
    }
}
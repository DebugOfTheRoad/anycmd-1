
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class DicUpdatedEvent : DomainEvent
    {
        #region Ctor
        public DicUpdatedEvent(DicBase source, IDicUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IDicUpdateInput Input { get; private set; }
    }
}

namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class PropertyRemovedEvent : DomainEvent
    {
        #region Ctor
        public PropertyRemovedEvent(PropertyBase source)
            : base(source)
        {
        }
        #endregion
    }
}

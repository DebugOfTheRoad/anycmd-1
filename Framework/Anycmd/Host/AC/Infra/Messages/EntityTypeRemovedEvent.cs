
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class EntityTypeRemovedEvent : DomainEvent
    {
        #region Ctor
        public EntityTypeRemovedEvent(EntityTypeBase source)
            : base(source)
        {
        }
        #endregion
    }
}

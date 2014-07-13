
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class ResourceTypeRemovedEvent : DomainEvent
    {
        #region Ctor
        public ResourceTypeRemovedEvent(ResourceTypeBase source)
            : base(source)
        {
        }
        #endregion
    }
}
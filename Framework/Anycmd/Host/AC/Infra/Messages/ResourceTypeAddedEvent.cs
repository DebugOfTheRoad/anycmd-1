
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class ResourceTypeAddedEvent : DomainEvent
    {
        #region Ctor
        public ResourceTypeAddedEvent(ResourceTypeBase source, IResourceCreateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
        }
        #endregion

        public IResourceCreateInput input { get; private set; }
    }
}

namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationRemovedEvent : DomainEvent
    {
        #region Ctor
        public OrganizationRemovedEvent(OrganizationBase source)
            : base(source)
        {
        }
        #endregion
    }
}

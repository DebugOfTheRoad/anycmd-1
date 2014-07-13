
namespace Anycmd.Host.AC.Infra.Messages
{
    using AC.ValueObjects;
    using Anycmd.AC.Infra;
    using Anycmd.Events;

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationUpdatedEvent : DomainEvent
    {
        #region Ctor
        public OrganizationUpdatedEvent(OrganizationBase source, IOrganizationUpdateInput input)
            : base(source)
        {
            if (input == null)
            {
                throw new System.ArgumentNullException("input");
            }
            this.Input = input;
        }
        #endregion

        public IOrganizationUpdateInput Input { get; private set; }
    }
}

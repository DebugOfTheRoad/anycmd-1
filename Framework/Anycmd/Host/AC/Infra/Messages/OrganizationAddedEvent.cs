
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class OrganizationAddedEvent : EntityAddedEvent<IOrganizationCreateInput>
    {
        #region Ctor
        public OrganizationAddedEvent(OrganizationBase source, IOrganizationCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}

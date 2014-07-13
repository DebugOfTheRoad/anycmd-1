
namespace Anycmd.Host.AC.Messages
{
    using Anycmd.AC;
    using Model;
    using ValueObjects;

    public class RoleAddedEvent : EntityAddedEvent<IRoleCreateInput>
    {
        #region Ctor
        public RoleAddedEvent(RoleBase source, IRoleCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}

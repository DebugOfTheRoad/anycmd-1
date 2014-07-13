
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class EntityTypeAddedEvent : EntityAddedEvent<IEntityTypeCreateInput>
    {
        #region Ctor
        public EntityTypeAddedEvent(EntityTypeBase source, IEntityTypeCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}

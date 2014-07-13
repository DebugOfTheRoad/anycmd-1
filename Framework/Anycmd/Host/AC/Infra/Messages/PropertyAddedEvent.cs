
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class PropertyAddedEvent : EntityAddedEvent<IPropertyCreateInput>
    {
        #region Ctor
        public PropertyAddedEvent(PropertyBase source, IPropertyCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}

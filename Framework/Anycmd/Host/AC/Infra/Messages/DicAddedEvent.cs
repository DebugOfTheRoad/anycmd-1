
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class DicAddedEvent : EntityAddedEvent<IDicCreateInput>
    {
        #region Ctor
        public DicAddedEvent(DicBase source, IDicCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}
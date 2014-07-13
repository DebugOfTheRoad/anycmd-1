
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class DicItemAddedEvent : EntityAddedEvent<IDicItemCreateInput>
    {
        #region Ctor
        public DicItemAddedEvent(DicItemBase source, IDicItemCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}
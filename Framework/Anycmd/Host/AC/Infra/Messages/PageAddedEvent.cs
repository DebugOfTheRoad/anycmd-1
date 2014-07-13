
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class PageAddedEvent : EntityAddedEvent<IPageCreateInput>
    {
        #region Ctor
        public PageAddedEvent(PageBase source, IPageCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}

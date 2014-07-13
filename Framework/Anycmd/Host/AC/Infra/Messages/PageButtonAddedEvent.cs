
namespace Anycmd.Host.AC.Infra.Messages
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class PageButtonAddedEvent : EntityAddedEvent<IPageButtonCreateInput>
    {
        #region Ctor
        public PageButtonAddedEvent(PageButtonBase source, IPageButtonCreateInput input)
            : base(source, input)
        {
        }
        #endregion
    }
}

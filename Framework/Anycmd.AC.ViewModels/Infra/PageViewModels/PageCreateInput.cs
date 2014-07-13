
namespace Anycmd.AC.Infra.ViewModels.PageViewModels
{
    using Host.AC.ValueObjects;
    using Model;

    /// <summary>
    /// 
    /// </summary>
    public class PageCreateInput : EntityCreateInput, IInputModel, IPageCreateInput
    {
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Tooltip { get; set; }
    }
}

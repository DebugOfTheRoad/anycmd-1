
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;

    /// <summary>
    /// 系统参数模型视图控制器<see cref="Common.Parameter"/>
    /// </summary>
    public class ParameterController : AnycmdController
    {
        public ParameterController()
        {
        }

        #region 视图

        [By("xuexs")]
        [Description("系统配置")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        #endregion
    }
}

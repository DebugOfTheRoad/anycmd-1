
namespace Anycmd.Web.Mvc
{
    using Anycmd.Host;
    using Exceptions;

    /// <summary>
    /// 所有控制器必须继承该类
    /// </summary>
    [AuthorizeFilter(Order = 20)]
    [CompressFilter(Order = 30)]
    [ExceptionFilter(Order = int.MaxValue)]
    public class AnycmdController : BaseController
    {
        protected EntityTypeState GetEntityType(string codespace, string entityTypeCode)
        {
            EntityTypeState entityTypeEntityType;
            if (!Host.EntityTypeSet.TryGetEntityType(codespace, entityTypeCode, out entityTypeEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            return entityTypeEntityType;
        }
    }
}

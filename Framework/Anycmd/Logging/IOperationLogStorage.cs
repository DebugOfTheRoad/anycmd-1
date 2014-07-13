
namespace Anycmd.Logging
{

    /// <summary>
    /// 操作日志暂存器。对于ASP.NET暂存在Http请求上下文中。
    /// <remarks>
    /// 暂不支持非ASP.NET
    /// </remarks>
    /// </summary>
    public interface IOperationLogStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operationLog"></param>
        void Set(FunctionDescriptor function);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        OperationLogBase Get();

        /// <summary>
        /// 
        /// </summary>
        void Remove();
    }
}

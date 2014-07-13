
namespace Anycmd.Host
{
    /// <summary>
    /// 系统参数。
    /// </summary>
    public interface IParameter
    {
        string GroupCode { get; }

        /// <summary>
        /// 参数编码。
        /// </summary>
        string Code { get; }

        /// <summary>
        /// 参数值。
        /// </summary>
        string Value { get; }
    }
}

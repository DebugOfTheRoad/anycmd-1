
namespace Anycmd.Host.EDI.Handlers
{

    /// <summary>
    /// 命令输入验证器。只验证输入。
    /// </summary>
    public interface IInputValidator {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        ProcessResult Validate(IMessage command);
    }
}

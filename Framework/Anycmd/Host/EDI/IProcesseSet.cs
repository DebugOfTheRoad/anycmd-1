
namespace Anycmd.Host.EDI
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 定义进程集合。
    /// <remarks>
    /// 注意：基础库的进程指的是命令接收服务器、命令执行器、命令分发器、数据交换平台Mis系统这基类进程。
    /// </remarks>
    /// </summary>
    public interface IProcesseSet : IEnumerable<ProcessDescriptor> {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        ProcessDescriptor this[Guid processID] { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processID"></param>
        /// <returns></returns>
        bool ContainsProcess(Guid processID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        bool TryGetProcess(Guid processID, out ProcessDescriptor process);
    }
}

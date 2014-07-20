
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 系统操作集合
    /// </summary>
    public interface IFunctionSet : IEnumerable<FunctionState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSystem"></param>
        /// <param name="resource"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        bool TryGetFunction(ResourceTypeState resource, string functionCode, out FunctionState function);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        bool TryGetFunction(Guid functionID, out FunctionState function);
    }
}

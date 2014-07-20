
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 权限应用系统集合。
    /// </summary>
    public interface IAppSystemSet : IEnumerable<AppSystemState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        AppSystemState SelfAppSystem { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSystemID"></param>
        /// <returns></returns>
        bool ContainsAppSystem(Guid appSystemID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSystemCode"></param>
        /// <returns></returns>
        bool ContainsAppSystem(string appSystemCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSystemID"></param>
        /// <param name="appSystem"></param>
        /// <returns></returns>
        bool TryGetAppSystem(Guid appSystemID, out AppSystemState appSystem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSystemCode"></param>
        /// <param name="appSystem"></param>
        /// <returns></returns>
        bool TryGetAppSystem(string appSystemCode, out AppSystemState appSystem);
    }
}

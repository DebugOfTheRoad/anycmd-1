
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 按钮集合
    /// </summary>
    public interface IButtonSet : IEnumerable<ButtonState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        bool ContainsButton(Guid buttonID);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonCode"></param>
        /// <returns></returns>
        bool ContainsButton(string buttonCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonID"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        bool TryGetButton(Guid buttonID, out ButtonState button);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonCode"></param>
        /// <param name="button"></param>
        /// <returns></returns>
        bool TryGetButton(string buttonCode, out ButtonState button);
    }
}

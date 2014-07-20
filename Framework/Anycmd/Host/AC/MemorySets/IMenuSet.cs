
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    public interface IMenuSet : IEnumerable<MenuState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuID"></param>
        /// <param name="menu"></param>
        /// <returns></returns>
        bool TryGetMenu(Guid menuID, out MenuState menu);
    }
}

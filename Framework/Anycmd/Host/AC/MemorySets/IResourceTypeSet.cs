﻿
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 资源上下文
    /// </summary>
    public interface IResourceTypeSet : IEnumerable<ResourceTypeState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resourceTypeID"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        bool TryGetResource(Guid resourceTypeID, out ResourceTypeState resource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSystem"></param>
        /// <param name="resourceCode"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        bool TryGetResource(AppSystemState appSystem, string resourceCode, out ResourceTypeState resource);
    }
}

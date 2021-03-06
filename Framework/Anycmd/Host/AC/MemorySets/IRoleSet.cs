﻿
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    public interface IRoleSet : IEnumerable<RoleState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        bool TryGetRole(Guid roleID, out RoleState role);

        IReadOnlyCollection<RoleState> GetDescendantRoles(RoleState role);
    }
}

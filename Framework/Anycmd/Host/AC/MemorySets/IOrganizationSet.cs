
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 组织结构集合
    /// </summary>
    public interface IOrganizationSet : IEnumerable<OrganizationState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationID"></param>
        /// <param name="oragnization"></param>
        /// <returns></returns>
        bool TryGetOrganization(Guid organizationID, out OrganizationState organization);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="organizationCode"></param>
        /// <param name="organization"></param>
        /// <returns></returns>
        bool TryGetOrganization(string organizationCode, out OrganizationState organization);
    }
}

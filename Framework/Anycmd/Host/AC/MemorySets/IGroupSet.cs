
namespace Anycmd.Host.AC.MemorySets
{
    using System;
    using System.Collections.Generic;

    public interface IGroupSet : IEnumerable<GroupState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        bool TryGetGroup(Guid groupID, out GroupState group);
    }
}

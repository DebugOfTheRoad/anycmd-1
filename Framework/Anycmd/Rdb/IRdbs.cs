
namespace Anycmd.Rdb
{
    using System;
    using System.Collections.Generic;

    public interface IRdbs : IEnumerable<RdbDescriptor>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbID"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        bool TryDb(Guid dbID, out RdbDescriptor db);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbID"></param>
        /// <returns></returns>
        bool ContainsDb(Guid dbID);
    }
}

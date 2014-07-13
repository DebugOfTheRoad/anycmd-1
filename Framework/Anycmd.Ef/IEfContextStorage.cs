
namespace Anycmd.Ef
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IEfContextStorage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void SetRepositoryContext(EfRepositoryContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        EfRepositoryContext GetRepositoryContext(string key);
    }
}
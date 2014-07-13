
namespace Anycmd
{
    using Host;
    using System;
    using System.Security.Principal;

    public interface IUserSession
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        AccountState GetAccount();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        AccountState GetContractor();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Guid GetAccountID();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        AppHost AppHost { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetData<T>(string key);
        /// <summary>
        /// 
        /// </summary>
        IPrincipal Principal { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void SetData(string key, object data);
    }
}

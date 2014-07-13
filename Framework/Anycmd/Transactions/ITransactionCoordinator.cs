using System;

namespace Anycmd.Transactions
{
    using Model;

    /// <summary>
    /// 定义事务协调器
    /// </summary>
    public interface ITransactionCoordinator : IUnitOfWork, IDisposable
    {
    }
}

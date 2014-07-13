
namespace Anycmd.Transactions
{
    using Model;

    /// <summary>
    /// 事务协调器工厂
    /// </summary>
    public static class TransactionCoordinatorFactory
    {
        /// <summary>
        /// 创建事务协调器并返回
        /// <remarks>如果每一个工作单元都支持分布式事务则其返回<see cref="DistributedTransactionCoordinator"/>
        /// ，否则返回<see cref="SuppressedTransactionCoordinator"/></remarks>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ITransactionCoordinator Create(params IUnitOfWork[] args)
        {
            bool ret = true;
            foreach (var arg in args)
                ret = ret && arg.DistributedTransactionSupported;
            if (ret)
                return new DistributedTransactionCoordinator(args);
            else
                return new SuppressedTransactionCoordinator(args);
        }
    }
}

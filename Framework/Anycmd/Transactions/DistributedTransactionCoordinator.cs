using System.Transactions;

namespace Anycmd.Transactions
{
    using Model;

    /// <summary>
    /// 分布式事务协调器。其依赖于微软的<see cref="TransactionScope"/>类实现分布式事务协调
    /// </summary>
    internal sealed class DistributedTransactionCoordinator : TransactionCoordinator
    {
        private readonly TransactionScope scope = new TransactionScope();

        public DistributedTransactionCoordinator(params IUnitOfWork[] unitOfWorks)
            : base(unitOfWorks)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                scope.Dispose();
        }


        public override void Commit()
        {
            base.Commit();
            scope.Complete();
        }
    }
}

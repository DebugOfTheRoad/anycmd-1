
namespace Anycmd.EDI.Queries.Ef
{
    using Anycmd.Ef;
    using ViewModels.BatchViewModels;

    /// <summary>
    /// 查询接口实现<see cref="IBatchQuery"/>
    /// </summary>
    public class BatchQuery : QueryBase, IBatchQuery
    {
        public BatchQuery(AppHost host)
            : base(host, "EDIEntities")
        {
        }
    }
}

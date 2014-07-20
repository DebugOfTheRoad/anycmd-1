﻿
namespace Anycmd.EDI.Queries.Ef
{
    using Anycmd.Ef;
    using ViewModels.StateCodeViewModels;

    /// <summary>
    /// 查询接口实现<see cref="IStateCodeQuery"/>
    /// </summary>
    public class StateCodeQuery : QueryBase, IStateCodeQuery
    {
        public StateCodeQuery(AppHost host)
            : base(host, "EDIEntities")
        {
        }
    }
}


namespace Anycmd.Host.AC.Infra
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 系统字典项集合
    /// </summary>
    public interface IDicSet : IEnumerable<DicState>
    {
        /// <summary>
        /// 
        /// </summary>
        Guid Id { get; }

        bool ContainsDic(Guid dicID);

        bool ContainsDic(string dicCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicID"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        bool TryGetDic(Guid dicID, out DicState dic);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicCode"></param>
        /// <param name="dic"></param>
        /// <returns></returns>
        bool TryGetDic(string dicCode, out DicState dic);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        IReadOnlyDictionary<string, DicItemState> GetDicItems(DicState dic);

        bool ContainsDicItem(Guid dicItemID);

        bool ContainsDicItem(DicState dic, string dicItemCode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicItemID"></param>
        /// <param name="dicItem"></param>
        /// <returns></returns>
        bool TryGetDicItem(Guid dicItemID, out DicItemState dicItem);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicState"></param>
        /// <param name="dicItemCode"></param>
        /// <param name="dicItem"></param>
        /// <returns></returns>
        bool TryGetDicItem(DicState dicState, string dicItemCode, out DicItemState dicItem);
    }
}

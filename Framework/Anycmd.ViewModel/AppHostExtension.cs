
namespace Anycmd
{
    using Exceptions;
    using Host;
    using System.Collections.Generic;

    /// <summary>
    /// 为AppHost提供扩展方法。<see cref="AppHost"/>
    /// </summary>
    public static class AppHostExtension
    {
        /// <summary>
        /// 根据系统字典将字典项码翻译为字典项名
        /// </summary>
        /// <param name="host"></param>
        /// <param name="dicCode"></param>
        /// <param name="dicItemCode"></param>
        /// <returns></returns>
        public static string Translate(this IAppHost host, string dicCode, int dicItemCode)
        {
            return Translate(host, dicCode, dicItemCode.ToString());
        }

        /// <summary>
        /// 根据系统字典将字典项码翻译为字典项名
        /// </summary>
        /// <param name="host"></param>
        /// <param name="dicCode"></param>
        /// <param name="dicItemCode"></param>
        /// <returns></returns>
        public static string Translate(this IAppHost host, string dicCode, bool dicItemCode)
        {
            return Translate(host, dicCode, dicItemCode.ToString());
        }

        /// <summary>
        /// 根据系统字典将字典项码翻译为字典项名
        /// </summary>
        /// <param name="host"></param>
        /// <param name="dicCode"></param>
        /// <param name="dicItemCode"></param>
        /// <returns></returns>
        public static string Translate(this IAppHost host, string dicCode, string dicItemCode)
        {
            DicState dic;
            if (host.DicSet.TryGetDic(dicCode, out dic))
            {
                IReadOnlyDictionary<string, DicItemState> dicItems = host.DicSet.GetDicItems(dic);
                if (dicItems.ContainsKey(dicItemCode))
                {
                    return dicItems[dicItemCode].Name;
                }
            }
            return dicItemCode;
        }

        /// <summary>
        /// 根据系统字典将字典项码翻译为字典项名
        /// </summary>
        /// <param name="host"></param>
        /// <param name="codespace"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="propertyCode"></param>
        /// <param name="dicItemCode"></param>
        /// <returns></returns>
        public static string Translate(this IAppHost host, string codespace, string entityTypeCode, string propertyCode, int dicItemCode)
        {
            return Translate(host, codespace, entityTypeCode, propertyCode, dicItemCode.ToString());
        }

        /// <summary>
        /// 根据系统字典将字典项码翻译为字典项名
        /// </summary>
        /// <param name="host"></param>
        /// <param name="codespace"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="propertyCode"></param>
        /// <param name="dicItemCode"></param>
        /// <returns></returns>
        public static string Translate(this IAppHost host, string codespace, string entityTypeCode, string propertyCode, bool dicItemCode)
        {
            return Translate(host, codespace, entityTypeCode, propertyCode, dicItemCode.ToString());
        }

        /// <summary>
        /// 根据系统字典将字典项码翻译为字典项名
        /// </summary>
        /// <param name="host"></param>
        /// <param name="codespace"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="propertyCode"></param>
        /// <param name="dicItemCode"></param>
        /// <returns></returns>
        public static string Translate(this IAppHost host, string codespace, string entityTypeCode, string propertyCode, string dicItemCode)
        {
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType(codespace, entityTypeCode, out entityType))
            {
                throw new CoreException("意外的实体类型" + codespace + entityTypeCode);
            }
            PropertyState property;
            if (!host.EntityTypeSet.TryGetProperty(entityType, propertyCode, out property))
            {
                return dicItemCode;
            }
            if (property.DicID.HasValue)
            {
                DicState dicState;
                if (!host.DicSet.TryGetDic(property.DicID.Value, out dicState))
                {
                    return dicItemCode;
                }
                DicItemState dicitem;
                if (host.DicSet.TryGetDicItem(dicState, dicItemCode, out dicitem))
                {
                    return dicitem.Name;
                }
            }
            return dicItemCode;
        }
    }
}

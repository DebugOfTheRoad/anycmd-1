
namespace Anycmd.EDI.MessageTransfers {
    using DataContracts;
    using Host.EDI.Info;
    using System.Linq;
    using Util;

    public static class KeyValueExtension {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [By("xuexs")]
        public static DataItem[] ToKeyValuePairs(this KeyValue[] source) {
            return source == null ? new DataItem[0] : source.Where(a => a != null).Select(a => new DataItem(a.Key, a.Value)).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [By("xuexs")]
        public static KeyValue[] ToKeyValues(this DataItem[] source) {
            if (source == null) {
                return new KeyValue[0];
            }
            return source.Select(a => new KeyValue(a.Key, a.Value)).ToArray();
        }
    }
}

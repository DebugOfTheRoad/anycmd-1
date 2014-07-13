
namespace Anycmd.EDI.InfoStringConverters {
    using Host.EDI.Info;
    using Model;
    using ServiceStack.Text;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    [Export(typeof(IInfoStringConverter))]
    public sealed class JsonInfoStringConverter : DisposableObject, IInfoStringConverter {
        private static readonly Guid id = new Guid("1B541773-7D71-4A16-9691-D8D160670B67");
        private static readonly string title = "json格式信息值转化器";
        private static readonly string description = "json格式信息值转化器";
        private static readonly string author = "xuexs";
        private static readonly DataItem[] emptyKeyValues = new DataItem[0];
        private static readonly string[] emptyStringArray = new string[0];

        /// <summary>
        /// 
        /// </summary>
        public string InfoFormat {
            get {
                return "json";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id {
            get { return id; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Title {
            get { return title; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description {
            get { return description; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Author {
            get { return author; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoString"></param>
        /// <returns></returns>
        public string[] ToStringArray(string infoString) {
            if (string.IsNullOrEmpty(infoString)) {
                return emptyStringArray;
            }
            string[] infoValues = JsonSerializer.DeserializeFromString<string[]>(infoString);
            if (infoValues.Length == 0) {
                return emptyStringArray;
            }

            return infoValues;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringArray"></param>
        /// <returns></returns>
        public string ToInfoString(string[] stringArray) {
            if (stringArray == null || stringArray.Length == 0) {
                return string.Empty;
            }

            return JsonSerializer.SerializeToString(stringArray);
        }

        /// <summary>
        /// 将给定的json字符串转化为键值对字典返回
        /// </summary>
        /// <param name="value">json格式的字符串</param>
        /// <returns>键值对字典</returns>
        public DataItem[] ToDataItems(string infoValueString) {
            if (string.IsNullOrEmpty(infoValueString)) {
                return emptyKeyValues;
            }
            Guid id;
            if (Guid.TryParse(infoValueString, out id)) {
                return new DataItem[] { new DataItem("Id", infoValueString) };
            }
            Dictionary<string, string> infoValues = JsonSerializer.DeserializeFromString<Dictionary<string, string>>(infoValueString);
            if (infoValues.Count == 0) {
                return emptyKeyValues;
            }

            var infoItems = new List<DataItem>();
            foreach (var item in infoValues) {
                infoItems.Add(new DataItem(item.Key, item.Value));
            }

            return infoItems.ToArray();
        }

        /// <summary>
        /// 根据给定的命令描述对象返回转化得到的信息字符串
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public string ToInfoString(IEnumerable<DataItem> infoItems) {
            if (infoItems == null) {
                return "{}";
            }
            Dictionary<string, string> infoValueDic = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in infoItems.Where(a => a != null && a.Key != null)) {
                if (!infoValueDic.ContainsKey(item.Key)) {
                    infoValueDic.Add(item.Key, item.Value);
                }
            }

            return JsonSerializer.SerializeToString(infoValueDic);
        }

        protected override void Dispose(bool disposing) {

        }
    }
}

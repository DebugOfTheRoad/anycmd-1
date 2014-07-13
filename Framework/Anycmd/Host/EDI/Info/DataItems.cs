
namespace Anycmd.Host.EDI.Info
{
    using Exceptions;
    using System;

    /// <summary>
    /// 数据项集合
    /// </summary>
    public sealed class DataItems {
        private DataItem[] dataItems;
        private string infoString;
        private readonly IInfoStringConverter converter;

        private DataItems() {
        }

        /// <summary>
        /// 命令信息项集合构造。两个参数不能同时为null
        /// </summary>
        /// <param name="dataItems">信息字符串所对应的数据项数组</param>
        /// <param name="infoString">数据项数组所对应的信息字符串</param>
        /// <param name="converter">信息格式：如json、xml</param>
        public DataItems(DataItem[] dataItems, string infoString, IInfoStringConverter converter) {
            if (dataItems == null && infoString == null) {
                throw new CoreException("dataItems和infoString不能同时为null");
            }
            if ((dataItems == null || infoString == null) && converter == null) {
                throw new ArgumentNullException("converter");
            }
            this.converter = converter;
            this.Items = dataItems;
            this.InfoString = infoString;
        }

        /// <summary>
        /// 数据项数组所对应的信息字符串
        /// </summary>
        public string InfoString {
            get {
                if (infoString == null) {
                    infoString = converter.ToInfoString(dataItems);
                }
                return infoString;
            }
            private set { infoString = value; }
        }

        /// <summary>
        /// 信息字符串所对应的数据项数组
        /// </summary>
        public DataItem[] Items {
            get {
                if (dataItems == null) {
                    dataItems = converter.ToDataItems(infoString);
                    if (dataItems == null) {
                        throw new CoreException("信息字符串转化器返回意外的null数组。");
                    }
                }
                return dataItems;
            }
            private set { dataItems = value; }
        }

        /// <summary>
        /// 判断集合是否是空的，即集合中没有命令信息项。
        /// </summary>
        public bool IsEmpty {
            get {
                if (dataItems == null) {
                    return Items.Length == 0;
                }
                return dataItems.Length == 0;
            }
        }
    }
}

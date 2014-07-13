
namespace Anycmd.DataContracts
{
    using System.Runtime.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class BodyData : IDto
    {
        private static readonly KeyValue[] empty = new KeyValue[0];

        public BodyData()
        {
            this.InfoID = empty;
            this.InfoValue = empty;
            this.Event = new EventData();
        }

        public BodyData(KeyValue[] infoID, KeyValue[] infoValue)
        {
            this.InfoID = infoID;
            this.InfoValue = infoValue;
            this.Event = new EventData();
        }

        /// <summary>
        /// 信息标识。Key、Value键值对数组，键为本体元素码。
        /// </summary>
        [DataMember(Order = 200)]
        public KeyValue[] InfoID { get; set; }

        /// <summary>
        /// 信息值。Key、Value键值对数组，键为本体元素码。
        /// </summary>
        [DataMember(Order = 210)]
        public KeyValue[] InfoValue { get; set; }

        /// <summary>
        /// 本体元素码数组，指示当前命令的ActionInfoResult响应项。对于get型命令来说null或空数组表示返回所有当前client有权get的字段，
        /// 对于非get型命令来说null或空数组表示不返回ActionInfoResult值。
        /// </summary>
        [DataMember(Order = 220)]
        public string[] QueryList { get; set; }

        /// <summary>
        /// 当MessageType为Event时有值。事件头有EventSourceType、EventSubjectCode、EventStateCode、EventReasonPhrase四个属性，它们用于帮助描述事件
        /// </summary>
        [DataMember]
        public EventData Event { get; set; }
    }
}


namespace Anycmd.EDI.Client
{

    /// <summary>
    /// 列头模型
    /// </summary>
    public class CommandColHeader
    {
        /// <summary>
        /// $MessageID 请求标识 命令消息标识
        /// </summary>
        public const string MESSAGE_ID = "$MessageID";
        /// <summary>
        /// $MessageType 请求类型
        /// </summary>
        public const string MESSAGE_TYPE = "$MessageType";
        /// <summary>
        /// $EventSourceType 事件源类型
        /// </summary>
        public const string EVENT_SOURCE_TYPE = "$EventSourceType";
        /// <summary>
        /// $EventSubjectCode 事件主题码
        /// </summary>
        public const string EVENT_SUBJECT_CODE = "$EventSubjectCode";
        /// <summary>
        /// $EventStateCode 事件状态码
        /// </summary>
        public const string EVENT_STATE_CODE = "$EventStateCode";
        /// <summary>
        /// $EventReasonPhrase 时间原因短语
        /// </summary>
        public const string EVENT_REASON_PHRASE = "$EventReasonPhrase";
        /// <summary>
        /// $Verb 本体动作码
        /// </summary>
        public const string VERB = "$Verb";
        /// <summary>
        /// $IsDumb 是否是哑命令
        /// </summary>
        public const string IS_DUMB = "$IsDumb";
        /// <summary>
        /// $TimeStamp 本体时间戳
        /// </summary>
        public const string TIME_STAMP = "$TimeStamp";
        /// <summary>
        /// $Ontology 本体码
        /// </summary>
        public const string ONTOLOGY = "$Ontology";
        /// <summary>
        /// $Version 协议版本号
        /// </summary>
        public const string VERSION = "$Version";
        /// <summary>
        /// $ServerTicks 服务器时间戳
        /// </summary>
        public const string SERVER_TICKS = "$ServerTicks";
        /// <summary>
        /// $LocalEntityID 建议信息标识
        /// </summary>
        public const string LOCAL_ENTITY_ID = "$LocalEntityID";
        /// <summary>
        /// $StateCode 响应状态码
        /// </summary>
        public const string STATE_CODE = "$StateCode";
        /// <summary>
        /// $ReasonPhrase 响应原因短语
        /// </summary>
        public const string REASON_PHRASE = "$ReasonPhrase";
        /// <summary>
        /// $Description 响应描述
        /// </summary>
        public const string DESCRIPTION = "$Description";
        /// <summary>
        /// $InfoIDKeys 信息标识键
        /// </summary>
        public const string INFO_ID_KEYS = "$InfoIDKeys";
        /// <summary>
        /// $InfoValueKeys 信息值键
        /// </summary>
        public const string INFO_VALUE_KEYS = "$InfoValueKeys";

        /// <summary>
        /// 列编码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 列名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 列默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// 是否隐藏该列。
        /// </summary>
        public bool IsHidden { get; set; }
    }
}

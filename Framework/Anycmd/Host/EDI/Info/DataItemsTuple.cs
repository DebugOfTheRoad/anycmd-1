
namespace Anycmd.Host.EDI.Info
{
	using Exceptions;

	/// <summary>
	/// 数据项集合对
	/// </summary>
	public sealed class DataItemsTuple {
		IInfoStringConverter converter;
		private string getElementString;
		private string[] getElement;

		private DataItemsTuple(
			DataItem[] dataIDItems, string idString,
			DataItem[] dataValueItems, string valueString,
			string[] getElement, string getElementString,
			string infoFormat) {
			if (dataIDItems == null && idString == null) {
				dataIDItems = new DataItem[0];
			}
			if (dataValueItems == null && valueString == null) {
				dataValueItems = new DataItem[0];
			}
			if (string.IsNullOrEmpty(infoFormat)) {
				throw new CoreException("infoFormat不能为空");
			}
			if (!NodeHost.Instance.InfoStringConverters.TryGetInfoStringConverter(infoFormat, out converter)) {
				throw new CoreException("意外的信息格式" + infoFormat);
			}
			this.InfoFormat = infoFormat;
			this.QueryList = getElement;
			this.QueryList = getElement;
			this.QueryListString = getElementString;
			this.IDItems = new DataItems(dataIDItems, idString, converter);
			this.ValueItems = new DataItems(dataValueItems, valueString, converter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataIDItems"></param>
		/// <param name="dataValueItems"></param>
		/// <param name = "getElement"></param>
		/// <param name="infoFormat"></param>
		/// <returns></returns>
		public static DataItemsTuple Create(
			DataItem[] dataIDItems,
			DataItem[] dataValueItems,
			string[] getElement,
			string infoFormat) {
			return new DataItemsTuple(dataIDItems, null, dataValueItems, null, getElement, null, infoFormat);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="idString"></param>
		/// <param name="valueString"></param>
		/// <param name = "getElementString"></param>
		/// <param name="infoFormat"></param>
		/// <returns></returns>
		public static DataItemsTuple Create(
			string idString,
			string valueString,
			string getElementString,
			string infoFormat) {
			return new DataItemsTuple(null, idString, null, valueString, null, getElementString, infoFormat);
		}

		/// <summary>
		/// 查看当前信息项集合对的信息格式
		/// </summary>
		public string InfoFormat { get; private set; }

		/// <summary>
		/// 本体元素码数组，指示当前命令的ActionInfoResult响应项。对于get型命令来说null或空数组表示返回所有当前client有权get的字段，
		/// 对于非get型命令来说null或空数组表示不返回ActionInfoResult值。
		/// </summary>
		public string[] QueryList {
			get {
				if (getElement == null && getElementString == null) {
					return null;
				}
				if (getElement == null) {
					getElement = converter.ToStringArray(getElementString);
				}
				return getElement;
			}
			private set { getElement = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string QueryListString {
			get {
				if (getElement == null && getElementString == null) {
					return null;
				}
				if (getElementString == null) {
					getElementString = converter.ToInfoString(getElement);
				}
				return getElementString;
			}
			private set { getElementString = value; }
		}

		/// <summary>
		/// 信息标识项。不可能为null
		/// </summary>
		public DataItems IDItems { get; private set; }

		/// <summary>
		/// 信息值项。不可能为null
		/// </summary>
		public DataItems ValueItems { get; private set; }
	}
}

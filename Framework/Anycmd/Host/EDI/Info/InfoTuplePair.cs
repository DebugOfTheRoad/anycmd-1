
namespace Anycmd.Host.EDI.Info
{
	using System;
	using System.Linq;

	/// <summary>
	/// 信息元组夫妻。信息元组夫妻由一个信息标识元组和一个信息值元组组成。
	/// </summary>
	public sealed class InfoTuplePair {
		private bool isSingleGuidGeted = false;
		private bool isSingleGuid = false;
		private InfoItem singleGuidItem = null;

		/// <summary>
		/// 构建信息元素。
		/// </summary>
		/// <param name="idItems"></param>
		/// <param name="valueItems"></param>
		public InfoTuplePair(InfoItem[] idItems, InfoItem[] valueItems) {
			this.IDTuple = idItems ?? new InfoItem[0];
			this.ValueTuple = valueItems ?? new InfoItem[0];
		}

		/// <summary>
		/// 查看当前信息元组是否基于单列Guid信息标识
		/// </summary>
		public bool IsSingleGuid {
			get {
				if (!isSingleGuidGeted) {
					isSingleGuidGeted = true;
					singleGuidItem = this.IDTuple.FirstOrDefault(a => string.Equals("Id", a.Element.Element.Code, StringComparison.OrdinalIgnoreCase));
					isSingleGuid = singleGuidItem != null;
				}
				return isSingleGuid;
			}
		}

		/// <summary>
		/// 如果当前信息元组基于单列Guid信息标识则返回单列Guid信息标识项，否则返回null
		/// </summary>
		public InfoItem SingleGuidItem {
			get {
				return IsSingleGuid ? singleGuidItem : null;
			}
		}

		/// <summary>
		/// 信息标识项。不可能为null
		/// </summary>
		public InfoItem[] IDTuple { get; private set; }

		/// <summary>
		/// 信息值项。不可能为null
		/// </summary>
		public InfoItem[] ValueTuple { get; private set; }
	}
}


namespace Anycmd.Host.EDI
{
	using Exceptions;
	using Hecp;
	using Host;
	using Host.AC.Infra;
	using System;
	using System.Collections.Generic;
	using System.Data;
	using Util;
	using elementCode = System.String;

	/// <summary>
	/// 本体元素描述对象。描述对象往往长久贮存在内存中。
	/// </summary>
	public sealed class ElementDescriptor
	{
		private bool _isCodeValue;
		private bool _isCodeValueDetected;
		private DbType _dbType;

		#region const string
		/// <summary>
		/// 数字自增标识编码
		/// </summary>
		public static readonly DbField IncrementIDCode = new DbField("IncrementID", System.Data.DbType.Int32, false, null);
		/// <summary>
		/// 实体的Guid标识编码
		/// </summary>
		public static readonly DbField IdElementCode = new DbField("Id", System.Data.DbType.Guid, false, null);
		/// <summary>
		/// 实体级权限
		/// </summary>
		public static readonly DbField EntityActionElementCode = new DbField("EntityAction", System.Data.DbType.String, true, 500);
		/// <summary>
		/// 实体元素级权限
		/// </summary>
		public static readonly DbField EntityElementActionElementCode = new DbField("EntityElementAction", System.Data.DbType.String, true, 500);
		/// <summary>
		/// 访问控制表政策
		/// </summary>
		public static readonly DbField EntityACLPolicyElementCode = new DbField("EntityACLPolicy", System.Data.DbType.String, true, 500);
		/// <summary>
		/// 
		/// </summary>
		public static readonly DbField EntityJavascriptElementCode = new DbField("EntityJavascript", System.Data.DbType.String, true, 1000);
		/// <summary>
		/// 创建时间编码
		/// </summary>
		public static readonly DbField CreateOnCode = new DbField("CreateOn", System.Data.DbType.DateTime, true, null);
		/// <summary>
		/// 最后修改时间编码
		/// </summary>
		public static readonly DbField ModifiedOnCode = new DbField("ModifiedOn", System.Data.DbType.DateTime, true, null);
		/// <summary>
		/// 创建者标识编码
		/// </summary>
		public static readonly DbField CreateUserIDCode = new DbField("CreateUserID", System.Data.DbType.Guid, true, null);
		/// <summary>
		/// 最后修改者标识编码
		/// </summary>
		public static readonly DbField ModifiedUserIDCode = new DbField("ModifiedUserID", System.Data.DbType.Guid, true, null);
		/// <summary>
		/// 创建节点标识编码
		/// </summary>
		public static readonly DbField CreateNodeIDCode = new DbField("CreateNodeID", System.Data.DbType.Guid, false, null);
		/// <summary>
		/// 最后修改节点标识编码
		/// </summary>
		public static readonly DbField ModifiedNodeIDCode = new DbField("ModifiedNodeID", System.Data.DbType.Guid, false, null);
		/// <summary>
		/// 创建命令标识编码
		/// </summary>
		public static readonly DbField CreateCommandIDCode = new DbField("CreateCommandID", System.Data.DbType.Guid, true, null);
		/// <summary>
		/// 最后修改命令标识编码
		/// </summary>
		public static readonly DbField ModifiedCommandIDCode = new DbField("ModifiedCommandID", System.Data.DbType.Guid, true, null);
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public static readonly Dictionary<elementCode, DbField> SystemElementCodes = new Dictionary<elementCode, DbField>(StringComparer.OrdinalIgnoreCase) {
			{IncrementIDCode.Name, IncrementIDCode},
			{IdElementCode.Name,IdElementCode},
			{EntityActionElementCode.Name,EntityActionElementCode},
			{EntityElementActionElementCode.Name,EntityElementActionElementCode},
			{EntityACLPolicyElementCode.Name,EntityACLPolicyElementCode},
			{EntityJavascriptElementCode.Name,EntityJavascriptElementCode},
			{CreateOnCode.Name,CreateOnCode},
			{ModifiedOnCode.Name,ModifiedOnCode},
			{CreateUserIDCode.Name,CreateUserIDCode},
			{ModifiedUserIDCode.Name,ModifiedUserIDCode},
			{CreateNodeIDCode.Name,CreateNodeIDCode},
			{ModifiedNodeIDCode.Name,ModifiedNodeIDCode},
			{CreateCommandIDCode.Name,CreateCommandIDCode},
			{ModifiedCommandIDCode.Name,ModifiedCommandIDCode}
		};

		private readonly IAppHost host;

		#region Ctor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="element"></param>
		public ElementDescriptor(IAppHost host, ElementState element)
		{
			this.host = host;
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			this.Element = element;
			if (!element.OType.TryParse(out _dbType))
			{
				throw new CoreException("意外的数据库类型" + element.OType);
			}
			string fieldName = element.Code;
			if (!string.IsNullOrEmpty(element.FieldCode))
			{
				fieldName = element.FieldCode;
			}
			this.FieldSchema = new DbField(fieldName, _dbType, element.Nullable, element.MaxLength);
			this.IsRuntimeElement = SystemElementCodes.ContainsKey(element.Code);
		}
		#endregion

		public IAppHost Host
		{
			get { return host; }
		}

		/// <summary>
		/// 
		/// </summary>
		public ElementState Element { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public DbField FieldSchema { get; private set; }

		#region IsCodeValue
		/// <summary>
		/// 是否是码值。码值对人类来说不易读，码值在展示给人类的时候往往需要翻译。
		/// </summary>
		public bool IsCodeValue
		{
			get
			{
				if (!_isCodeValueDetected)
				{
					_isCodeValueDetected = true;
					_isCodeValue = false;
					if (this.Ontology.Ontology.IsOrganizationalEntity && this.Element.Code.Equals("ZZJGM", StringComparison.OrdinalIgnoreCase))
					{
						_isCodeValue = true;
					}
					else if (this == this.Ontology.CreateNodeIDElement)
					{
						_isCodeValue = true;
					}
					else if (this == this.Ontology.ModifiedNodeIDElement)
					{
						_isCodeValue = true;
					}
					else if (this.Element.InfoDicID.HasValue)
					{
						_isCodeValue = true;
					}
				}
				return _isCodeValue;
			}
			internal set
			{
				_isCodeValue = value;
			}
		}
		#endregion

		/// <summary>
		/// 本体
		/// </summary>
		public OntologyDescriptor Ontology
		{
			get
			{
				return Host.Ontologies[this.Element.OntologyID];
			}
		}

		/// <summary>
		/// 数据库列
		/// </summary>
		public ElementDataSchema DataSchema
		{
			get
			{
				return this.Ontology.EntityProvider.GetElementDataSchema(this);
			}
		}

		/// <summary>
		/// 是否是运行时元素。True表示是，False不是。
		/// </summary>
		public bool IsRuntimeElement { get; private set; }

		/// <summary>
		/// 1，“配置验证”项通过的条件是：首先，主题元素对应编码的字段在数据库中是存在的；其次，主题元素上配置的长度是小于等于数据库中对应字段的长度的。
		/// </summary>
		public bool IsConfigValid
		{
			get
			{
				if (DataSchema == null)
				{
					return false;
				}
				bool isValid = true;
				if (Element.MaxLength > DataSchema.MaxLength)
				{
					isValid = false;
				}
				else if (DataSchema == null)
				{
					isValid = false;
				}

				return isValid;
			}
		}

		#region TranslateValue
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public string TranslateValue(string value)
		{
			if (!this.IsCodeValue)
			{
				return value;
			}
			// 是否能成功翻译，如果不能成功翻译则会返回原始值。
			// 翻译组织结构码为组织结构名
			if (this.Ontology.Ontology.IsOrganizationalEntity
				&& this.Element.Code.Equals("ZZJGM", StringComparison.OrdinalIgnoreCase))
			{
				OrganizationState org;
				return Host.OrganizationSet.TryGetOrganization(value, out org) ? org.Name : "非法的无法翻译的组织结构码";
			}
			// 翻译节点标识为节点名
			else if (this == this.Ontology.CreateNodeIDElement)
			{
				NodeDescriptor node;
				return Host.Nodes.TryGetNodeByID(value, out node) ? node.Node.Name : "非法的无法翻译的节点标识";
			}
			// 翻译节点标识为节点名
			else if (this == this.Ontology.ModifiedNodeIDElement)
			{
				NodeDescriptor node;
				if (Host.Nodes.TryGetNodeByID(value, out node))
				{
					return node.Node.Name;
				}
				else
				{
					return "非法的无法翻译的节点标识";
				}
			}
			// 翻译字典项编码为字典项名称
			else if (this.Element.InfoDicID.HasValue)
			{
				if (!string.IsNullOrWhiteSpace(value))
				{
					InfoDicState infoDic;
					if (!Host.InfoDics.TryGetInfoDic(this.Element.InfoDicID.Value, out infoDic))
					{
						return value;
					}
					InfoDicItemState infoDicItem;
					return Host.InfoDics.TryGetInfoDicItem(infoDic, value, out infoDicItem) ? infoDicItem.Name : "非法的无法翻译的字典值";
				}
				else
				{
					return string.Empty;
				}
			}
			else
			{
				return value;
			}
		}
		#endregion

		/// <summary>
		/// 获取给定节点的节点元素级权限
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public IReadOnlyDictionary<Verb, NodeElementActionState> GetActions(NodeDescriptor node)
		{
			return Host.Nodes.GetNodeElementActions(node, this);
		}

		public override int GetHashCode()
		{
			return Element.Id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			var value = obj as ElementDescriptor;
			if (value == null)
			{
				return false;
			}
			return this.Element == value.Element || this.Element.Id == value.Element.Id;
		}
	}
}

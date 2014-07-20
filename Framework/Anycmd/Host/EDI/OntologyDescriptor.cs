
namespace Anycmd.Host.EDI
{
	using Anycmd.EDI;
	using Exceptions;
	using Hecp;
	using Host;
	using Host.EDI.Handlers;
	using System.Collections.Generic;
	using System.Linq;

	/// <summary>
	/// 本体描述器。内部的不可变的。描述对象往往长久贮存在内存中。
	/// <para>
	/// 本体（Ontology）是对一类实体的分类，如教师是一类实体，学生是一类实体。平台使用固定的唯一的编码来识别
	/// 每一类实体，将教师编码为JS，学生编码为XS，这种编码是不区分大小写的字符串，JS和XS就是“本体码（OntologyCode）”
	/// ，关于本体码在下文有专门的一章可进一步了解。本体完整的涵盖一类实体的所有数据项，而不同的节点对同一类实体所
	/// 关注的数据项往往不同，每一个节点所关注的通常是一类实体所有数据项的一个子集，这个子集我们称为话题（Topic），
	/// 有时Topic也称作数据上下文（Data Context），如教育一线通系统关心JS的手机号码而不关心教师的出生年月，教育一线通
	/// 的上下文就是“发手机短信”这件事，数据上下文采用分类法进行识别。
	/// </para>
	/// </summary>
	public sealed class OntologyDescriptor
	{
		#region Private Fields
		private IMessageProvider _messageProvider;
		private IEntityProvider _entityProvider;
		private ElementDescriptor _incrementID = null;
		private ElementDescriptor _idElement = null;
		private ElementDescriptor _entityAction = null;
		private ElementDescriptor _entityElementAction = null;
		private ElementDescriptor _entityACLPolicyElement = null;
		private ElementDescriptor _entityJavascriptElement = null;
		private ElementDescriptor _createOn = null;
		private ElementDescriptor _modifiedOn = null;
		private ElementDescriptor _createUserID = null;
		private ElementDescriptor _modifiedUserID = null;
		private ElementDescriptor _createNodeID = null;
		private ElementDescriptor _modifiedNodeID = null;
		private ElementDescriptor _createCommandID = null;
		private ElementDescriptor _modifiedCommandID = null;
		#endregion

		private readonly IAppHost host;

		#region Ctor
		public OntologyDescriptor(IAppHost host, OntologyState ontology)
		{
			this.host = host;
			this.Ontology = ontology;
		}
		#endregion

		#region public Properties
		public IAppHost Host
		{
			get { return host; }
		}

		/// <summary>
		/// 本体
		/// </summary>
		public OntologyState Ontology { get; private set; }

		#region MessageProvider
		/// <summary>
		/// 命令提供程序
		/// </summary>
		public IMessageProvider MessageProvider
		{
			get
			{
				if (_messageProvider == null)
				{
					if (!Host.MessageProviders.TryGetMessageProvider(
						this.Ontology.MessageProviderID, out _messageProvider))
					{
						throw new CoreException("意外的命令提供程序ID" + this.Ontology.MessageProviderID);
					}
				}
				return _messageProvider;
			}
		}
		#endregion

		#region EntityProvider
		/// <summary>
		/// 数据提供程序
		/// </summary>
		public IEntityProvider EntityProvider
		{
			get
			{
				if (_entityProvider == null)
				{
					if (!Host.EntityProviders.TryGetEntityProvider(
						this.Ontology.EntityProviderID, out _entityProvider))
					{
						throw new CoreException("意外的数据提供程序ID" + this.Ontology.EntityProviderID);
					}
				}
				return _entityProvider;
			}
		}
		#endregion

		/// <summary>
		/// 本本体的本体元素字典
		/// </summary>
		public IReadOnlyDictionary<string, ElementDescriptor> Elements
		{
			get
			{
				return Host.Ontologies.GetElements(this);
			}
		}

		/// <summary>
		/// 信息组
		/// </summary>
		public IEnumerable<IInfoGroup> InfoGroups
		{
			get
			{
				return Host.Ontologies.GetInfoGroups(this);
			}
		}

		/// <summary>
		/// 键为Verb，值为ActionIsAllow。不区分键的大小写
		/// </summary>
		public IReadOnlyDictionary<Verb, ActionState> Actions
		{
			get
			{
				return Host.Ontologies.GetActons(this);
			}
		}

		/// <summary>
		/// key为实践主题码
		/// </summary>
		public IReadOnlyDictionary<string, TopicState> Topics
		{
			get
			{
				return Host.Ontologies.GetEventSubjects(this);
			}
		}

		public IEnumerable<ProcessDescriptor> Processes
		{
			get
			{
				return Host.Processs.Where(a => a.Process.OntologyID == this.Ontology.Id);
			}
		}

		/// <summary>
		/// key为组织结构码
		/// </summary>
		public IReadOnlyDictionary<OrganizationState, OntologyOrganizationState> Organizations
		{
			get
			{
				return Host.Ontologies.GetOntologyOrganizations(this);
			}
		}

		#region 运行时本体元素/系统本体元素
		/*
		 * 本体是领域、是边界、是讨论问题时设定的上下文。如果“教师”是一个本体，那么“教师”这个本体（领域、边界、上下文）下有些什么呢？ 
		 * 有教师作为一个“人”的基本属性、有教师作为一个“教育人”的基本属性，有教师在“电子世界”的属性，这些都是教师的“数据属性”。
		 * 这里的“人”、“教育人”、“电子世界”是什么？它们也是本体，当“教师”本体和“基础库”本体作用的时候基础库又赋予了教师一些“系统属性”，
		 * 这些系统属性与教师的其它属性没有任何本质差别。 
		 */
		/// <summary>
		/// 自增数字标识
		/// </summary>
		public ElementDescriptor IncrementIDElement
		{
			get
			{
				if (_incrementID == null)
				{
					_incrementID = this.Elements[ElementDescriptor.IncrementIDCode.Name];
				}
				return _incrementID;
			}
		}

		/// <summary>
		/// 主键本体元素<remarks>系统使用该本体元素作为单列Guid信息标识</remarks>
		/// </summary>
		public ElementDescriptor IdElement
		{
			get
			{
				if (_idElement == null)
				{
					_idElement = this.Elements[ElementDescriptor.IdElementCode.Name];
				}
				return _idElement;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ElementDescriptor EntityActionElement
		{
			get
			{
				if (_entityAction == null)
				{
					_entityAction = this.Elements[ElementDescriptor.EntityActionElementCode.Name];
				}
				return _entityAction;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ElementDescriptor EntityElementActionElement
		{
			get
			{
				if (_entityElementAction == null)
				{
					_entityElementAction = this.Elements[ElementDescriptor.EntityElementActionElementCode.Name];
				}
				return _entityElementAction;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ElementDescriptor EntityACLPolicyElement
		{
			get
			{
				if (_entityACLPolicyElement == null)
				{
					_entityACLPolicyElement = this.Elements[ElementDescriptor.EntityACLPolicyElementCode.Name];
				}
				return _entityACLPolicyElement;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ElementDescriptor EntityJavascriptElement
		{
			get
			{
				if (_entityJavascriptElement == null)
				{
					_entityJavascriptElement = this.Elements[ElementDescriptor.EntityJavascriptElementCode.Name];
				}
				return _entityJavascriptElement;
			}
		}

		/// <summary>
		/// 创建时间本体元素
		/// </summary>
		public ElementDescriptor CreateOnElement
		{
			get
			{
				if (_createOn == null)
				{
					_createOn = this.Elements[ElementDescriptor.CreateOnCode.Name];
				}
				return _createOn;
			}
		}

		/// <summary>
		/// 最后修改时间本体元素
		/// </summary>
		public ElementDescriptor ModifiedOnElement
		{
			get
			{
				if (_modifiedOn == null)
				{
					_modifiedOn = this.Elements[ElementDescriptor.ModifiedOnCode.Name];
				}
				return _modifiedOn;
			}
		}

		/// <summary>
		/// 创建人。当在Mis系统执行命令的时候才涉及到操作人
		/// </summary>
		public ElementDescriptor CreateUserIDElement
		{
			get
			{
				if (_createUserID == null)
				{
					_createUserID = this.Elements[ElementDescriptor.CreateUserIDCode.Name];
				}
				return _createUserID;
			}
		}

		/// <summary>
		/// 最后修改人。当在Mis系统执行命令的时候才涉及到操作人
		/// </summary>
		public ElementDescriptor ModifiedUserIDElement
		{
			get
			{
				if (_modifiedUserID == null)
				{
					_modifiedUserID = this.Elements[ElementDescriptor.ModifiedUserIDCode.Name];
				}
				return _modifiedUserID;
			}
		}

		/// <summary>
		/// 创建节点本体元素
		/// </summary>
		public ElementDescriptor CreateNodeIDElement
		{
			get
			{
				if (_createNodeID == null)
				{
					_createNodeID = this.Elements[ElementDescriptor.CreateNodeIDCode.Name];
				}
				return _createNodeID;
			}
		}

		/// <summary>
		/// 最后修改节点本体元素
		/// </summary>
		public ElementDescriptor ModifiedNodeIDElement
		{
			get
			{
				if (_modifiedNodeID == null)
				{
					_modifiedNodeID = this.Elements[ElementDescriptor.ModifiedNodeIDCode.Name];
				}
				return _modifiedNodeID;
			}
		}

		/// <summary>
		/// 创建命令本体元素
		/// </summary>
		public ElementDescriptor CreateCommandIDElement
		{
			get
			{
				if (_createCommandID == null)
				{
					_createCommandID = this.Elements[ElementDescriptor.CreateCommandIDCode.Name];
				}
				return _createCommandID;
			}
		}

		/// <summary>
		/// 最后修改命令本体元素
		/// </summary>
		public ElementDescriptor ModifiedCommandIDElement
		{
			get
			{
				if (_modifiedCommandID == null)
				{
					_modifiedCommandID = this.Elements[ElementDescriptor.ModifiedCommandIDCode.Name];
				}
				return _modifiedCommandID;
			}
		}
		#endregion

		#endregion

		public IReadOnlyCollection<ArchiveState> GetArchives()
		{
			return Host.Ontologies.GetArchives(this);
		}

		public override int GetHashCode()
		{
			return this.Ontology.Id.GetHashCode();
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
			if (!(obj is OntologyDescriptor))
			{
				return false;
			}
			var left = this;
			var right = (OntologyDescriptor)obj;

			return left.Ontology == right.Ontology;
		}

		public static bool operator ==(OntologyDescriptor a, OntologyDescriptor b)
		{
			// If both are null, or both are same instance, return true.
			if (System.Object.ReferenceEquals(a, b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return a.Equals(b);
		}

		public static bool operator !=(OntologyDescriptor a, OntologyDescriptor b)
		{
			return !(a == b);
		}
	}
}

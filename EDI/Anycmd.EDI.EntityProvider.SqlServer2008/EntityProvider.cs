
namespace Anycmd.EDI.EntityProvider.SqlServer2008
{
	using Exceptions;
	using Host.AC.Infra;
	using Host.EDI;
	using Host.EDI.Handlers;
	using Host.EDI.Info;
	using Logging;
	using Query;
	using Rdb;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.Data.SqlClient;
	using System.Linq;
	using System.Text;
	using Util;

	/// <summary>
	/// 基于SqlServer2008的数据提供程序
	/// </summary>
	[Export(typeof(IEntityProvider))]
	public sealed class EntityProvider : IEntityProvider
	{
		private static readonly Guid id = new Guid("A108FCAD-17A1-4F3C-A3F2-804131A0855A");
		private static readonly string title = "数据提供程序SqlServer2008";
		private static readonly string description = "使用SqlServer2008R2数据库";
		private static readonly string author = "xuexs";
		private static readonly Dictionary<ElementDescriptor, ElementDataSchema>
			_elementDataSchemaDic = new Dictionary<ElementDescriptor, ElementDataSchema>();
		private static readonly Dictionary<OntologyDescriptor, RdbDescriptor> _dbDic = new Dictionary<OntologyDescriptor, RdbDescriptor>();
		private static object _locker = new object();

		private readonly ISqlFilterStringBuilder filterStringBuilder = new SqlFilterStringBuilder();

		#region 属性
		/// <summary>
		/// 插件标识
		/// </summary>
		public Guid Id
		{
			get { return id; }
		}

		/// <summary>
		/// 标题
		/// </summary>
		public string Title
		{
			get { return title; }
		}

		/// <summary>
		/// 描述
		/// </summary>
		public string Description
		{
			get { return description; }
		}

		/// <summary>
		/// 作者。如xuexs
		/// </summary>
		public string Author
		{
			get { return author; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return this.Title; }
		}

		/// <summary>
		/// 
		/// </summary>
		public BuiltInResourceKind BuiltInResourceKind
		{
			get { return BuiltInResourceKind.EntityDbProvider; }
		}
		#endregion

		#region 本体元素数据模式
		/// <summary>
		/// 查看给定元素的本体元素数据模式
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public ElementDataSchema GetElementDataSchema(ElementDescriptor element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			ElementDataSchema dataSchema;
			if (_elementDataSchemaDic.TryGetValue(element, out dataSchema))
			{
				return dataSchema;
			}
			else
			{
				var db = this.GetEntityDb(element.Ontology);
				DbTableColumn column;
				string columnID = string.Format("[{0}][{1}][{2}]", element.Ontology.Ontology.EntitySchemaName, element.Ontology.Ontology.EntityTableName, element.Element.FieldCode);
				if (!NodeHost.Instance.AppHost.DbTableColumns.TryGetDbTableColumn(db, columnID, out column))
				{
					var msg = "实体库中不存在" + columnID + "列";
					LoggingService.Error(msg);
					return null;
				}
				dataSchema = new ElementDataSchema(column);
				_elementDataSchemaDic.Add(element, dataSchema);
				return dataSchema;
			}
		}
		#endregion

		#region 进程地址
		public string GetEntityDataSource(OntologyDescriptor ontology)
		{
			return this.GetEntityDb(ontology).IsLocalhost ? "localhost"
					: this.GetEntityDb(ontology).Database.DataSource;
		}
		#endregion

		#region 执行命令
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public ProcessResult ExecuteCommand(DbCommand command)
		{
			try
			{
				if (command == null)
				{
					throw new ArgumentNullException("command");
				}
				if (command.Ontology == null)
				{
					throw new ArgumentNullException("ontology");
				}
				if (command.Client == null)
				{
					throw new ArgumentNullException("client");
				}
				Sql sqlObj = new Sql(command.Ontology, command.Client.Id.ToString(), command.CommandID, command.ActionType, command.LocalEntityID, command.InfoValue);
				if (!sqlObj.IsValid)
				{
					return new ProcessResult(false, Status.ExecuteFail, sqlObj.Description);
				}
				else
				{
					if (!command.IsDumb)
					{
						int n = this.GetEntityDb(command.Ontology).ExecuteNonQuery(sqlObj.Text, sqlObj.Parameters);
						if (n == 0)
						{
							if (command.ActionType == DbActionType.Insert)
							{
								return new ProcessResult(false, Status.AlreadyExist, "已经创建");
							}
							else
							{
								return new ProcessResult(false, Status.ExecuteFail, "目标记录不存在");
							}
						}
						else if (n > 1)
						{
							return new ProcessResult(new CoreException("Id:" + command.CommandID + ",意外的影响行数" + n.ToString()));
						}
					}
					return new ProcessResult(true, Status.ExecuteOk, "执行成功");
				}
			}
			catch (Exception ex)
			{
				LoggingService.Error(ex);
				return new ProcessResult(ex);
			}
		}
		#endregion

		#region ArchiveEntityDb
		/// <summary>
		/// 
		/// </summary>
		/// <param name="archive"></param>
		public void Archive(OntologyDescriptor ontology, IArchive archive)
		{
			var entityDb = this.GetEntityDb(ontology).Database;
			var archiveDb = this.GetArchiveDb(ontology, archive);
			if (archiveDb.Database.CatalogName.Equals(entityDb.CatalogName, StringComparison.OrdinalIgnoreCase))
			{
				throw new CoreException("归档库的数据库名不能与本体库相同");
			}
			// 创建归档库
			archiveDb.Create(this.GetEntityDb(ontology), HostConfig.Instance.EntityArchivePath);
			string archiveTableName = string.Format(
				"{0}.{1}.{2}", archiveDb.Database.CatalogName, ontology.Ontology.EntitySchemaName, ontology.Ontology.EntityTableName);
			string sql =
@"if OBJECT_ID('" + archiveTableName + "') IS NULL select * into " + archiveTableName
+ " from " + ontology.Ontology.EntityTableName;
			// 执行select into归档数据
			this.GetEntityDb(ontology).ExecuteNonQuery(sql, null);
		}
		#endregion

		#region DropArchivedEntityDb
		/// <summary>
		/// 删除归档数据
		/// </summary>
		/// <param name="archive"></param>
		public void DropArchive(ArchiveState archive)
		{
			var _catalogName = string.Format(
							"Archive{0}{1}_{2}",
							archive.Ontology.Ontology.Code,
							archive.ArchiveOn.ToString("yyyyMMdd"),
							archive.NumberID.ToString());
			if (_catalogName.Equals(this.GetEntityDb(archive.Ontology).Database.CatalogName, StringComparison.OrdinalIgnoreCase))
			{
				throw new CoreException("归档库的数据库名不能与本体库相同");
			}
			string sql =
@"IF EXISTS ( SELECT  1
			FROM    master..sysdatabases
			WHERE   name = '" + _catalogName + @"' ) 
	ALTER DATABASE " + _catalogName + @" SET SINGLE_USER WITH ROLLBACK IMMEDIATE
IF EXISTS ( SELECT  1
			FROM    master..sysdatabases
			WHERE   name = '" + _catalogName + @"' ) 
	ALTER DATABASE " + _catalogName + @" SET MULTI_USER
IF EXISTS ( SELECT  1
			FROM    master..sysdatabases
			WHERE   name = '" + _catalogName + @"' ) 
	DROP DATABASE " + _catalogName + "";
			this.GetEntityDb(archive.Ontology).ExecuteNonQuery(sql, null);
		}
		#endregion

		#region GetTopTwo
		public TowInfoTuple GetTopTwo(
			OntologyDescriptor ontology, IEnumerable<InfoItem> infoIDs, OrderedElementSet selectElements)
		{
			return GetTop2InfoItemSet(ontology, this.GetEntityDb(ontology), infoIDs, selectElements);
		}

		private TowInfoTuple GetTop2InfoItemSet(
			ArchiveState archive, IEnumerable<InfoItem> infoIDs, OrderedElementSet selectElements)
		{
			return GetTop2InfoItemSet(archive.Ontology, this.GetArchiveDb(archive.Ontology, archive), infoIDs, selectElements);
		}
		#endregion

		#region Get
		/// <summary>
		/// 获取给定本体码和存储标识的本节点的数据
		/// <remarks>本节点通常是中心节点</remarks>
		/// </summary>
		/// <param name="ontology">本体</param>
		/// <param name="id">存储标识</param>
		/// <returns>数据记录，表现为字典形式，键是数据元素编码值是相应数据元素对应的数据项值</returns>
		public InfoItem[] Get(
			OntologyDescriptor ontology, string localEntityID, OrderedElementSet selectElements)
		{
			var topTwo = GetTopTwo(ontology, new InfoItem[] { InfoItem.Create(ontology.IdElement, localEntityID) }, selectElements);
			if (topTwo.BothHasValue || topTwo.BothNoValue)
			{
				return new InfoItem[0];
			}
			else
			{
				return topTwo.SingleInfoTuple;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="archive"></param>
		/// <param name="localEntityID"></param>
		/// <param name="selectElements"></param>
		/// <returns></returns>
		public InfoItem[] Get(
			ArchiveState archive, string localEntityID, OrderedElementSet selectElements)
		{
			var topTwo = GetTop2InfoItemSet(archive, new InfoItem[] { InfoItem.Create(archive.Ontology.IdElement, localEntityID) }, selectElements);
			if (topTwo.BothHasValue || topTwo.BothNoValue)
			{
				return new InfoItem[0];
			}
			else
			{
				return topTwo.SingleInfoTuple;
			}
		}
		#endregion

		#region GetList
		/// <summary>
		/// 获取给定实体标识列表中的标识标识的每一个实体元组
		/// </summary>
		/// <param name="ontology"></param>
		/// <param name="selectElements"></param>
		/// <param name="entityIDs"></param>
		/// <returns></returns>
		public DataTuple GetList(OntologyDescriptor ontology, OrderedElementSet selectElements, List<string> entityIDs)
		{
			if (ontology == null)
			{
				throw new ArgumentNullException("ontology");
			}
			if (selectElements == null)
			{
				throw new ArgumentNullException("selectElements");
			}
			if (entityIDs == null || entityIDs.Count == 0)
			{
				throw new ArgumentException("entityID");
			}
			StringBuilder ids = new StringBuilder("(");
			int len = ids.Length;
			foreach (var id in entityIDs)
			{
				if (ids.Length != len)
				{
					ids.Append(",");
				}
				ids.Append("'").Append(id).Append("'");
			}
			ids.Append(")");
			var tableName = ontology.Ontology.EntityTableName;
			if (selectElements == null)
			{
				selectElements = new OrderedElementSet();
			}
			if (selectElements.Count == 0)
			{
				selectElements.Add(ontology.Elements[ontology.IdElement.Element.Code]);
			}
			StringBuilder sqlQuery = new StringBuilder("select ");
			int l = sqlQuery.Length;
			foreach (var item in selectElements)
			{
				if (sqlQuery.Length != l)
				{
					sqlQuery.Append(",");
				}
				sqlQuery.Append("t.[").Append(item.Element.Code).Append("]");
			}
			sqlQuery.Append(" from [").Append(tableName).Append("] as t where t.[")
				.Append(ontology.IdElement.Element.Code).Append("] in ").Append(ids.ToString());// TODO:参数化EntityIDs
			List<object[]> list = new List<object[]>();
			var reader = this.GetEntityDb(ontology).ExecuteReader(sqlQuery.ToString());
			while (reader.Read())
			{
				object[] values = new object[selectElements.Count];
				for (int i = 0; i < selectElements.Count; i++)
				{
					values[i] = reader.GetValue(i);
				}
				list.Add(values);
			}
			reader.Close();

			return new DataTuple(selectElements, list.ToArray());
		}
		#endregion

		#region GetPlist
		/// <summary>
		/// 按照组织结构分页获取指定节点、本体的数据
		/// <remarks>
		/// 如果传入的组织结构为空则表示获取全部组织结构的数据
		/// </remarks>
		/// </summary>
		/// <param name="db">本体</param>
		/// <param name="zzjgm">组织结构码列表</param>
		/// <param name="includedescendants">True表示包括子级，False不包括</param>
		/// <param name="filters">过滤器列表</param>
		/// <param name="pageIndex">页索引，零基索引，第一页对应0</param>
		/// <param name="pageSize">页尺寸</param>
		/// <param name="sortField">排序字段</param>
		/// <param name="sortOrder">排序方向</param>
		/// <param name="total">总记录数</param>
		/// <returns></returns>
		public DataTuple GetPlist(
			OntologyDescriptor ontology,
			OrderedElementSet selectElements,
			List<FilterData> filters,
			PagingInput pagingData)
		{
			return this.GetPlistInfoItems(
				ontology, this.GetEntityDb(ontology),
				selectElements,
				filters, pagingData);
		}

		public DataTuple GetPlist(
			ArchiveState archive,
			OrderedElementSet selectElements,
			List<FilterData> filters,
			PagingInput pagingData)
		{
			return this.GetPlistInfoItems(
				archive.Ontology, this.GetArchiveDb(archive.Ontology, archive),
				selectElements,
				filters, pagingData);
		}
		#endregion

		#region private method
		#region GetTop2InfoItemSet
		/// <summary>
		/// 根据给定的本体码和信息标识获取本节点两条数据
		/// </summary>
		/// <param name="ontology">本体</param>
		/// <param name="infoIDs">多列联合信息标识字典，键必须不区分大小写</param>
		/// <param name="selectElements">选择元素</param>
		/// <returns></returns>
		private TowInfoTuple GetTop2InfoItemSet(OntologyDescriptor ontology,
			RdbDescriptor db, IEnumerable<InfoItem> infoIDs, OrderedElementSet selectElements)
		{
			if (infoIDs == null || infoIDs.Count() == 0)
			{
				return new TowInfoTuple(null, null);
			}

			StringBuilder sb = new StringBuilder();
			List<SqlParameter> sqlParameters = new List<SqlParameter>();
			List<ElementDescriptor> elementList = selectElements.ToList();
			sb.Append("select top 2 ");
			int l = sb.Length;
			foreach (var element in elementList)
			{
				if (sb.Length != l)
				{
					sb.Append(",");
				}
				sb.Append("t.[").Append(element.Element.FieldCode).Append("]");
			}
			sb.Append(" from [").Append(ontology.Ontology.EntityTableName).Append("] as t where");
			if (ontology.Ontology.IsLogicalDeletionEntity)
			{
				sb.Append(" t.DeletionStateCode=0 ");
			}
			else
			{
				sb.Append(" 1=1 ");
			}
			foreach (var element in infoIDs)
			{
				sb.Append(" and t.[").Append(element.Element.Element.FieldCode)
					.Append("]=").Append("@").Append(element.Element.Element.FieldCode);
				object obj = element.Value;
				if (obj == null)
				{
					obj = DBNull.Value;
				}
				sqlParameters.Add(new SqlParameter(element.Element.Element.FieldCode, obj));
			}
			List<InfoItem> infoValue1 = new List<InfoItem>();
			List<InfoItem> infoValue2 = new List<InfoItem>();
			using (var reader = db.ExecuteReader(sb.ToString(), sqlParameters.ToArray()))
			{
				if (reader.Read())
				{
					for (int i = 0; i < elementList.Count; i++)
					{
						infoValue1.Add(InfoItem.Create(elementList[i], reader.GetValue(i).ToString()));
					}
				}
				if (reader.Read())
				{
					for (int i = 0; i < elementList.Count; i++)
					{
						infoValue2.Add(InfoItem.Create(elementList[i], reader.GetValue(i).ToString()));
					}
				}
				reader.Close();
			}

			return new TowInfoTuple(infoValue1.ToArray(), infoValue2.ToArray());
		}
		#endregion

		#region GetPlistInfoItems
		/// <summary>
		/// 根据组织结构获取给定节点和本体的数据，如果传入的组织结构为空表示获取本节点的数据
		/// <remarks>本节点通常是中心节点</remarks>
		/// </summary>
		/// <param name="db">模型</param>
		/// <param name="zzjgm">组织机构码</param>
		/// <param name="includedescendants"></param>
		/// <param name="filters"></param>
		/// <param name="selectElements">sql select语句的选择列集合</param>
		/// <param name="pageIndex">页索引，起始为0</param>
		/// <param name="pageSize">分页大小</param>
		/// <param name="sortField">排序字段</param>
		/// <param name="sortOrder">排序方向:asc或desc</param>
		/// <param name="total">总记录数</param>
		/// <returns>
		/// 数据记录列表，数据记录表现为字典形式，键是数据元素编码值是相应数据元素对应的数据项值
		/// </returns>
		private DataTuple GetPlistInfoItems(
			OntologyDescriptor ontology,
			RdbDescriptor db, OrderedElementSet selectElements, List<FilterData> filters,
			PagingInput pagingData)
		{
			if (string.IsNullOrEmpty(pagingData.sortField))
			{
				pagingData.sortField = "IncrementID";
			}
			if (string.IsNullOrEmpty(pagingData.sortOrder))
			{
				pagingData.sortOrder = "asc";
			}

			var elements = ontology.Elements;
			if (filters != null)
			{
				for (int i = 0; i < filters.Count; i++)
				{
					var filter = filters[i];
					if (elements.ContainsKey(filter.field))
					{
						// TODO:根据数据属性优化查询，比如对于身份证件号来说如果输入的值长度
						// 为20或18的话可以将like替换为等于
						filter.type = "string";
						var element = elements[filter.field];
						if (element.Element.IsEnabled != 1)
						{
							continue;
						}
						if (element.Element.InfoDicID.HasValue)
						{
							filter.comparison = "eq";
						}
						else
						{
							filter.comparison = "like";
						}
					}
					else
					{
						filters.RemoveAt(i);
					}
				}
			}

			var tableName = ontology.Ontology.EntityTableName;
			StringBuilder sbSqlPredicate = new StringBuilder();
			var l = sbSqlPredicate.Length;

			List<SqlParameter> pQueryList = new List<SqlParameter>();
			List<SqlParameter> pFilters;
			var filterString = filterStringBuilder.FilterString(filters, null, out pFilters);
			if (!string.IsNullOrEmpty(filterString))
			{
				foreach (var pFilter in pFilters)
				{
					object obj = pFilter.Value;
					if (obj == null)
					{
						obj = DBNull.Value;
					}
					pQueryList.Add(new SqlParameter(pFilter.ParameterName, obj));
				}
				if (sbSqlPredicate.Length != l)
				{
					sbSqlPredicate.Append(" and ");
				}
				sbSqlPredicate.Append(filterString);
			}

			string sqlPredicateString = string.Empty;
			if (sbSqlPredicate.Length > 0)
			{
				sqlPredicateString = sbSqlPredicate.ToString();
			}
			StringBuilder sqlText = new StringBuilder();
			OrderedElementSet elementList;
			if (selectElements == null || selectElements.Count == 0)
			{
				elementList = new OrderedElementSet();
				elementList.Add(ontology.Elements["id"]);
			}
			else
			{
				elementList = selectElements;
			}
			sqlText.Append("SELECT TOP {0} ");
			int len = sqlText.Length;

			foreach (var element in elementList)
			{
				if (sqlText.Length != len)
				{
					sqlText.Append(",");
				}
				sqlText.Append("[").Append(element.Element.FieldCode).Append("]");
			}

			sqlText.Append(" FROM (SELECT ROW_NUMBER() OVER(ORDER BY {1} {2}) AS RowNumber,");
			len = sqlText.Length;

			foreach (var element in elementList)
			{
				if (sqlText.Length != len)
				{
					sqlText.Append(",");
				}
				sqlText.Append("[").Append(element.Element.FieldCode).Append("]");
			}

			sqlText.Append(" FROM {3} where ");
			if (ontology.Ontology.IsLogicalDeletionEntity)
			{
				sqlText.Append("DeletionStateCode = 0");
			}
			else
			{
				sqlText.Append("1 = 1");
			}
			if (!string.IsNullOrEmpty(sqlPredicateString))
			{
				sqlText.Append(" and ").Append(sqlPredicateString);
			}
			sqlText.Append(") a WHERE a.RowNumber > {4}");
			string sqlQuery = string.Format(
				sqlText.ToString(),
				pagingData.pageSize.ToString(),
				pagingData.sortField,
				pagingData.sortOrder,
				tableName,
				(pagingData.SkipCount).ToString());

			pagingData.Count(() =>
			{
				string where = ontology.Ontology.IsLogicalDeletionEntity ? "where DeletionStateCode = 0" : "";
				string sqlCount = string.Format("select count(1) from {0} {1}", tableName, where);
				if (!string.IsNullOrEmpty(sqlPredicateString))
				{
					sqlCount = sqlCount + " and " + sqlPredicateString;
				}
				return (int)db.ExecuteScalar(
					sqlCount, pQueryList.Select(p => ((ICloneable)p).Clone()).ToArray());
			});

			List<object[]> list = new List<object[]>();
			var reader = db.ExecuteReader(sqlQuery, pQueryList.ToArray());
			while (reader.Read())
			{
				object[] values = new object[elementList.Count];
				for (int i = 0; i < elementList.Count; i++)
				{
					values[i] = reader.GetValue(i);
				}
				list.Add(values);
			}
			reader.Close();

			return new DataTuple(elementList, list.ToArray());
		}
		#endregion

		#region GetEntityDb
		/// <summary>
		/// 
		/// </summary>
		private RdbDescriptor GetEntityDb(OntologyDescriptor ontology)
		{
			if (!_dbDic.ContainsKey(ontology))
			{
				lock (_locker)
				{
					if (!_dbDic.ContainsKey(ontology))
					{
						RdbDescriptor db;
						if (!NodeHost.Instance.AppHost.Rdbs.TryDb(ontology.Ontology.EntityDatabaseID, out db))
						{
							throw new CoreException("意外的数据库ID" + ontology.Ontology.EntityDatabaseID.ToString());
						}
						_dbDic.Add(ontology, db);
					}
				}
			}
			return _dbDic[ontology];
		}
		#endregion

		#region GetArchiveDb
		private RdbDescriptor GetArchiveDb(OntologyDescriptor ontology, IArchive archive)
		{
			var entityDb = this.GetEntityDb(ontology).Database;
			var _catalogName = string.Format(
							"Archive{0}{1}_{2}",
							ontology.Ontology.Code,
							archive.ArchiveOn.ToString("yyyyMMdd"),
							archive.NumberID.ToString());
			var datasource = archive.DataSource;
			var userID = archive.UserID;
			var password = archive.Password;
			if (string.IsNullOrEmpty(datasource))
			{
				datasource = entityDb.DataSource;
			}
			if (string.IsNullOrEmpty(userID))
			{
				userID = entityDb.UserID;
			}
			if (string.IsNullOrEmpty(password))
			{
				password = entityDb.Password;
			}

			return new RdbDescriptor(NodeHost.Instance.AppHost, new RDatabase
			{
				CatalogName = _catalogName,
				CreateBy = archive.CreateBy,
				CreateOn = DateTime.Now,
				CreateUserID = archive.CreateUserID,
				DataSource = datasource,
				Description = ontology.Ontology.Name + "归档",
				Id = Guid.NewGuid(),
				IsTemplate = false,
				Password = password,
				Profile = entityDb.Profile,
				ProviderName = entityDb.ProviderName,
				RdbmsType = archive.RdbmsType,
				UserID = userID
			});
		}
		#endregion
		#endregion

		#region 内部类 Sql
		/// <summary>
		/// sql语句模型
		/// </summary>
		private sealed class Sql
		{
			OntologyDescriptor ontology;
			string localEntityID;
			string clientID;
			string commandID;
			InfoItem[] infoValue;
			private bool _isValid = true;
			private string _description = string.Empty;
			private readonly StringBuilder _text = new StringBuilder();
			private readonly Dictionary<string, SqlParameter> _parameters = new Dictionary<string, SqlParameter>(StringComparer.OrdinalIgnoreCase);

			private Sql() { }

			/// <summary>
			/// 
			/// </summary>
			/// <param name="command"></param>
			public Sql(OntologyDescriptor ontology, string clientID, string commandID, DbActionType actionType, string localEntityID, InfoItem[] infoValue)
			{
				this.ontology = ontology;
				this.localEntityID = localEntityID;
				this.infoValue = infoValue;
				this.clientID = clientID;
				this.commandID = commandID;
				if (ontology == null
					|| string.IsNullOrEmpty(localEntityID))
				{
					this.IsValid = false;
					this.Description = "命令信息标识或信息值为空或本体为空或本地标识为空";
					throw new ArgumentNullException("command");
				}
				// 无需把switch流程选择逻辑重构掉，因为actionType枚举不存在变化
				switch (actionType)
				{
					case DbActionType.Insert:
						if (infoValue == null || infoValue.Length == 0)
						{
							this.Description = "命令信息值为空";
							break;
						}
						BuildInsertSql();
						break;
					case DbActionType.Update:
						if (infoValue == null || infoValue.Length == 0)
						{
							this.Description = "命令信息值为空";
							break;
						}
						BuildUpdateSql();
						break;
					case DbActionType.Delete:
						BuildDeleteSql();
						break;
					default:
						this.IsValid = false;
						this.Description = "意外的不能执行的动作码" + actionType.ToString();
						break;
				}
			}

			#region 属性
			/// <summary>
			/// 
			/// </summary>
			public bool IsValid
			{
				get
				{
					return _isValid;
				}
				private set
				{
					_isValid = value;
				}
			}

			/// <summary>
			/// 
			/// </summary>
			public string Description
			{
				get { return _description; }
				private set
				{
					_description = value;
				}
			}

			/// <summary>
			/// sql语句
			/// </summary>
			public string Text
			{
				get
				{
					return _text.ToString();
				}
			}

			/// <summary>
			/// 参数数组
			/// </summary>
			public object[] Parameters
			{
				get
				{
					return _parameters.Values.ToArray();
				}
			}
			#endregion

			#region private methods
			#region BuildInsertSql
			/// <summary>
			/// 构建create语句
			/// </summary>
			/// <returns></returns>
			private void BuildInsertSql()
			{
				IList<InfoItem> valueItems = new List<InfoItem>();
				valueItems.Add(InfoItem.Create(ontology.IdElement, localEntityID));
				valueItems.Add(InfoItem.Create(ontology.CreateNodeIDElement, clientID));
				valueItems.Add(InfoItem.Create(ontology.ModifiedNodeIDElement, clientID));
				valueItems.Add(InfoItem.Create(ontology.CreateCommandIDElement, commandID));
				valueItems.Add(InfoItem.Create(ontology.ModifiedCommandIDElement, commandID));
				foreach (var item in infoValue)
				{
					valueItems.Add(item);
				}

				_text.Append("insert into ").Append(ontology.Ontology.EntityTableName).Append("(");
				int l1 = _text.Length;
				foreach (var valueItem in valueItems)
				{
					if (_text.Length > l1)
					{
						_text.Append(",");
					}
					_text.Append(valueItem.Element.Element.FieldCode);
				}
				_text.Append(") SELECT ");
				l1 = _text.Length;
				foreach (var valueItem in valueItems)
				{
					if (_text.Length > l1)
					{
						_text.Append(",");
					}
					_text.Append("@").Append(valueItem.Element.Element.FieldCode);
				}
				_text.Append(" WHERE   NOT EXISTS ( SELECT ");
				l1 = _text.Length;
				foreach (var valueItem in valueItems)
				{
					if (_text.Length > l1)
					{
						_text.Append(",");
					}
					_text.Append(valueItem.Element.Element.FieldCode);
				}
				_text.Append(" FROM ").Append(ontology.Ontology.EntityTableName);
				_text.Append(@" WHERE  Id = @Id);");
				foreach (var value in valueItems)
				{
					object obj = value.Value;
					if (value.Element.DataSchema.TypeName.StartsWith("date"))
					{
						DateTime t;
						if (!DateTime.TryParse(value.Value, out t))
						{
							this.IsValid = false;
							this.Description += value.Key + "非法的日期格式" + value.Value + ";";
						}
						if (t < SystemTime.MinDate)
						{
							t = SystemTime.MinDate;
						}
						else if (t > SystemTime.MaxDate)
						{
							t = SystemTime.MaxDate;
						}
						obj = t;
					}
					if (obj == null)
					{
						obj = DBNull.Value;
					}
					_parameters.Add(value.Key, new SqlParameter(value.Key, obj));
				}
			}
			#endregion

			#region BuildUpdateSql
			/// <summary>
			/// 构建update语句
			/// </summary>
			/// <returns></returns>
			private void BuildUpdateSql()
			{
				IList<InfoItem> valueItems = new List<InfoItem>();
				// 添加触发器关注字段
				valueItems.Add(InfoItem.Create(ontology.ModifiedOnElement, DateTime.Now.ToString()));
				valueItems.Add(InfoItem.Create(ontology.ModifiedNodeIDElement, clientID));
				valueItems.Add(InfoItem.Create(ontology.ModifiedCommandIDElement, commandID));
				foreach (var item in infoValue)
				{
					valueItems.Add(item);
				}
				_text.Append("update [").Append(ontology.Ontology.EntityTableName).Append("] set ");
				int l2 = _text.Length;
				foreach (var infoItem in valueItems)
				{
					if (infoItem.Element != ontology.IdElement)
					{
						if (_text.Length > l2)
						{
							_text.Append(",");
						}
						_text.Append(infoItem.Element.Element.FieldCode).Append("=");
						this.Append(infoItem);
					}
				}

				_text.Append(" where Id=");
				this.Append(InfoItem.Create(ontology.IdElement, localEntityID));
			}
			#endregion

			#region BuildDeleteSql
			/// <summary>
			/// 构建delete语句
			/// </summary>
			/// <returns></returns>
			private void BuildDeleteSql()
			{
				if (ontology.Ontology.IsLogicalDeletionEntity)
				{
					_text.Append("update [").Append(
						ontology.Ontology.EntityTableName).Append("] set DeletionStateCode=1, ")
						.Append(ontology.ModifiedNodeIDElement.Element.FieldCode)
						.Append("='" + clientID + "',")
						.Append(ontology.ModifiedCommandIDElement.Element.FieldCode)
						.Append("='" + commandID + "' where Id=");
				}
				else
				{
					_text.Append("delete [").Append(ontology.Ontology.EntityTableName).Append("] where Id=");
				}
				this.Append(InfoItem.Create(ontology.IdElement, localEntityID));
			}
			#endregion

			#region PrepareSql
			/// <summary>
			/// 
			/// </summary>
			/// <param name="sb"></param>
			/// <param name="value"></param>
			/// <param name="parameters"></param>
			private void Append(InfoItem value)
			{
				if (!_parameters.ContainsKey(value.Key))
				{
					object obj = value.Value;
					if (value.Element.DataSchema.TypeName.StartsWith("date"))
					{
						DateTime t;
						if (!DateTime.TryParse(value.Value, out t))
						{
							this.IsValid = false;
							this.Description += value.Key + "非法的日期格式" + value.Value + ";";
						}
						if (t < SystemTime.MinDate)
						{
							t = SystemTime.MinDate;
						}
						else if (t > SystemTime.MaxDate)
						{
							t = SystemTime.MaxDate;
						}
						obj = t;
					}
					if (obj == null)
					{
						obj = DBNull.Value;
					}
					_parameters.Add(value.Key, new SqlParameter(value.Key, obj));
					_text.Append("@").Append(value.Key);
				}
			}
			#endregion

			#endregion
		}
		#endregion
	}
}

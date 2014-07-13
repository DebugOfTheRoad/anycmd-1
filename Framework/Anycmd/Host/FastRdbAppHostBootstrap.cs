
namespace Anycmd.Host
{
    using AC;
    using AC.Identity;
    using AC.Infra;
    using Anycmd.Rdb;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Reflection;
    using System.Text;

    public class FastRdbAppHostBootstrap : IAppHostBootstrap
    {
        /// <summary>
        /// 数据库连接字符串引导库连接字符串
        /// </summary>
        private string _bootConnString = ConfigurationManager.AppSettings["BootDbConnString"];

        /// <summary>
        /// 数据库连接字符串引导库连接字符串
        /// </summary>
        public string BootConnString { get { return _bootConnString; } }

        private readonly AppHost host;
        private DataSet ds = null;
        private List<string> tableNames = new List<string>();

        public FastRdbAppHostBootstrap()
        {
            this.host = AppHost.Instance;
        }

        public FastRdbAppHostBootstrap(AppHost host)
        {
            this.host = host;
        }

        private DataTable this[string tableName]
        {
            get
            {
                if (ds == null)
                {
                    lock (this)
                    {
                        if (ds == null)
                        {
                            ds = new DataSet();
                            using (var conn = new SqlConnection(this.BootConnString))
                            {
                                if (conn.State != ConnectionState.Open)
                                {
                                    conn.Open();
                                }
                                StringBuilder sb = new StringBuilder();
                                Append(sb, "Parameter", "select * from [Parameter];");
                                Append(sb, "ResourceType", "select * from [ResourceType];");
                                Append(sb, "AppSystem", "select * from [AppSystem];");
                                Append(sb, "Function", "select * from [Function];");
                                Append(sb, "Dic", "select * from [Dic];");
                                Append(sb, "DicItem", "select * from [DicItem];");
                                Append(sb, "EntityType", "select * from [EntityType];");
                                Append(sb, "Property", "select * from [Property];");
                                Append(sb, "Organization", "select * from [Organization];");
                                Append(sb, "Menu", "select * from [Menu];");
                                Append(sb, "Button", "select * from [Button];");
                                Append(sb, "Group", "select * from [Group] where TypeCode='AC';");
                                Append(sb, "Page", "select * from [Page];");
                                Append(sb, "PageButton", "select * from [PageButton];");
                                Append(sb, "PrivilegeBigram", "select * from [PrivilegeBigram] where SubjectType!='Account';");// 查询非账户主体的权限记录，账户主体的权限记录在会话中查询
                                Append(sb, "Role", "select * from [Role];");
                                Append(sb, "RDatabase", "select * from [RDatabase] order by CatalogName;");
                                Append(sb, "Account", "select a.* from [Account] as a join DeveloperID as d on a.Id=d.Id;");
                                SqlCommand cmd = conn.CreateCommand();
                                cmd.CommandText = sb.ToString();
                                cmd.CommandType = CommandType.Text;
                                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                                sda.Fill(ds);
                            }
                        }
                    }
                }

                return ds.Tables[tableNames.IndexOf(tableName)];
            }
        }

        private void Append(StringBuilder sb, string tableName, string sql)
        {
            sb.Append(sql);
            tableNames.Add(tableName);
        }
        public List<IParameter> GetParameters()
        {
            var list = new List<IParameter>();
            foreach (DataRow row in this["Parameter"].Rows)
            {
                list.Add(new Parameter((Guid)row["Id"], row["GroupCode"].ToString(), row["CategoryCode"].ToString(), row["Code"].ToString(), row["Name"].ToString(), row["Value"].ToString()));
            }
            return list;
        }

        public IList<RDatabase> GetAllRDatabases()
        {
            var list = new List<RDatabase>();
            foreach (DataRow row in this["RDatabase"].Rows)
            {
                list.Add(new RDatabase
                {
                    CatalogName = (string)row["CatalogName"],
                    Id = (Guid)row["Id"],
                    IsTemplate = (bool)row["IsTemplate"],
                    DataSource = (string)row["DataSource"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    Profile = row["Profile"] == DBNull.Value ? null : row["Profile"].ToString(),
                    Password = (string)row["Password"],
                    RdbmsType = (string)row["RdbmsType"],
                    UserID = (string)row["UserID"],
                    ProviderName = (string)row["ProviderName"]
                });
            }
            return list;
        }

        public IList<DbTableColumn> GetTableColumns(RdbDescriptor db)
        {
            return host.GetRequiredService<IRdbMetaDataService>().GetTableColumns(db);
        }

        public IList<DbTable> GetDbTables(RdbDescriptor db)
        {
            return host.GetRequiredService<IRdbMetaDataService>().GetDbTables(db);
        }

        public IList<DbViewColumn> GetViewColumns(RdbDescriptor db)
        {
            return host.GetRequiredService<IRdbMetaDataService>().GetViewColumns(db);
        }

        public IList<DbView> GetDbViews(RdbDescriptor db)
        {
            return host.GetRequiredService<IRdbMetaDataService>().GetDbViews(db);
        }

        public IList<Organization> GetOrganizations()
        {
            var list = new List<Organization>();
            foreach (DataRow row in this["Organization"].Rows)
            {
                var item = new Organization
                {
                    Id = (Guid)row["Id"],
                    ParentCode = row["ParentCode"] == DBNull.Value ? null : row["ParentCode"].ToString(),
                    CategoryCode = row["CategoryCode"] == DBNull.Value ? null : row["CategoryCode"].ToString(),
                    PrincipalID = row["PrincipalID"] == DBNull.Value ? null : (Guid?)row["PrincipalID"],
                    Code = (string)row["Code"],
                    ShortName = row["ShortName"] == DBNull.Value ? null : row["ShortName"].ToString(),
                    Name = (string)row["Name"],
                    PrivilegeState = row["PrivilegeState"] == DBNull.Value ? null : (int?)row["PrivilegeState"],
                    OuterPhone = row["OuterPhone"] == DBNull.Value ? null : row["OuterPhone"].ToString(),
                    InnerPhone = row["InnerPhone"] == DBNull.Value ? null : row["InnerPhone"].ToString(),
                    Fax = row["Fax"] == DBNull.Value ? null : row["Fax"].ToString(),
                    Postalcode = row["Postalcode"] == DBNull.Value ? null : row["Postalcode"].ToString(),
                    Address = row["Address"] == DBNull.Value ? null : row["Address"].ToString(),
                    WebPage = row["WebPage"] == DBNull.Value ? null : row["WebPage"].ToString(),
                    LeadershipID = row["LeadershipID"] == DBNull.Value ? null : (Guid?)row["LeadershipID"],
                    AssistantLeadershipID = row["AssistantLeadershipID"] == DBNull.Value ? null : (Guid?)row["AssistantLeadershipID"],
                    ManagerID = row["ManagerID"] == DBNull.Value ? null : (Guid?)row["ManagerID"],
                    AssistantManagerID = row["AssistantManagerID"] == DBNull.Value ? null : (Guid?)row["AssistantManagerID"],
                    AccountingID = row["AccountingID"] == DBNull.Value ? null : (Guid?)row["AccountingID"],
                    CashierID = row["CashierID"] == DBNull.Value ? null : (Guid?)row["CashierID"],
                    FinancialID = row["FinancialID"] == DBNull.Value ? null : (Guid?)row["FinancialID"],
                    Bank = row["Bank"] == DBNull.Value ? null : row["Bank"].ToString(),
                    BankAccount = row["BankAccount"] == DBNull.Value ? null : row["BankAccount"].ToString(),
                    DeletionStateCode = (int)row["DeletionStateCode"],
                    IsEnabled = (int)row["IsEnabled"],
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    SortCode = (int)row["SortCode"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString()
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<AppSystem> GetAllAppSystems()
        {
            var list = new List<AppSystem>();
            foreach (DataRow row in this["AppSystem"].Rows)
            {
                var item = new AppSystem
                {
                    Id = (Guid)row["Id"],
                    AllowDelete = (int)row["AllowDelete"],
                    AllowEdit = (int)row["AllowEdit"],
                    Code = (string)row["Code"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    ImageUrl = row["ImageUrl"] == DBNull.Value ? null : row["ImageUrl"].ToString(),
                    IsEnabled = (int)row["IsEnabled"],
                    Name = row["Name"] == DBNull.Value ? null : row["Name"].ToString(),
                    PrincipalID = (Guid)row["PrincipalID"],
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    SSOAuthAddress = row["SSOAuthAddress"] == DBNull.Value ? null : row["SSOAuthAddress"].ToString()
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Button> GetAllButtons()
        {
            var list = new List<Button>();
            foreach (DataRow row in this["Button"].Rows)
            {
                var item = new Button
                {
                    Id = (Guid)row["Id"],
                    CategoryCode = row["CategoryCode"] == DBNull.Value ? null : row["CategoryCode"].ToString(),
                    Code = (string)row["Code"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    IsEnabled = (int)row["IsEnabled"],
                    Name = (string)row["Name"],
                    SortCode = (int)row["SortCode"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];
                list.Add(item);
            }
            return list;
        }

        public IList<Dic> GetAllDics()
        {
            var list = new List<Dic>();
            foreach (DataRow row in this["Dic"].Rows)
            {
                var item = new Dic
                {
                    Id = (Guid)row["Id"],
                    Code = (string)row["Code"],
                    IsEnabled = (int)row["IsEnabled"],
                    Name = (string)row["Name"],
                    SortCode = (int)row["SortCode"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString()
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<DicItem> GetAllDicItems()
        {
            var list = new List<DicItem>();
            foreach (DataRow row in this["DicItem"].Rows)
            {
                var item = new DicItem
                {
                    Id = (Guid)row["Id"],
                    Code = (string)row["Code"],
                    DicID = (Guid)row["DicID"],
                    Name = (string)row["Name"],
                    IsEnabled = (int)row["IsEnabled"],
                    SortCode = (int)row["SortCode"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString()
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<EntityType> GetAllEntityTypes()
        {
            var list = new List<EntityType>();
            foreach (DataRow row in this["EntityType"].Rows)
            {
                var item = new EntityType
                {
                    Id = (Guid)row["Id"],
                    Code = (string)row["Code"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    DeveloperID = (Guid)row["DeveloperID"],
                    Codespace = (string)row["Codespace"],
                    DatabaseID = (Guid)row["DatabaseID"],
                    IsOrganizational = (bool)row["IsOrganizational"],
                    Name = (string)row["Name"],
                    SchemaName = row["SchemaName"] == DBNull.Value ? null : row["SchemaName"].ToString(),
                    TableName = row["TableName"] == DBNull.Value ? null : row["TableName"].ToString(),
                    EditHeight = (int)row["EditHeight"],
                    EditWidth = (int)row["EditWidth"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Property> GetAllProperties()
        {
            var list = new List<Property>();
            foreach (DataRow row in this["Property"].Rows)
            {
                var item = new Property
                {
                    Id = (Guid)row["Id"],
                    Code = (string)row["Code"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    Name = (string)row["Name"],
                    DicID = row["DicID"] == DBNull.Value ? null : (Guid?)row["DicID"],
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    EntityTypeID = (Guid)row["EntityTypeID"],
                    ForeignPropertyID = row["ForeignPropertyID"] == DBNull.Value ? null : (Guid?)row["ForeignPropertyID"],
                    GroupCode = row["GroupCode"] == DBNull.Value ? null : row["GroupCode"].ToString(),
                    GuideWords = row["GuideWords"] == DBNull.Value ? null : row["GuideWords"].ToString(),
                    InputType = row["InputType"] == DBNull.Value ? null : row["InputType"].ToString(),
                    IsDetailsShow = (bool)row["IsDetailsShow"],
                    IsDeveloperOnly = (bool)row["IsDeveloperOnly"],
                    IsInput = (bool)row["IsInput"],
                    IsTotalLine = (bool)row["IsTotalLine"],
                    MaxLength = row["MaxLength"] == DBNull.Value ? null : (int?)row["MaxLength"],
                    Tooltip = row["Tooltip"] == DBNull.Value ? null : row["Tooltip"].ToString()
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Function> GetAllFunctions()
        {
            var list = new List<Function>();
            foreach (DataRow row in this["Function"].Rows)
            {
                var item = new Function
                {
                    Id = (Guid)row["Id"],
                    Code = (string)row["Code"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    IsEnabled = (int)row["IsEnabled"],
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    IsManaged = (bool)row["IsManaged"],
                    DeveloperID = (Guid)row["DeveloperID"],
                    ResourceTypeID = (Guid)row["ResourceTypeID"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Group> GetAllGroups()
        {
            var list = new List<Group>();
            foreach (DataRow row in this["Group"].Rows)
            {
                var item = new Group
                {
                    Id = (Guid)row["Id"],
                    Name = (string)row["Name"],
                    ShortName = row["ShortName"] == DBNull.Value ? null : row["ShortName"].ToString(),
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    IsEnabled = (int)row["IsEnabled"],
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    CategoryCode = row["CategoryCode"] == DBNull.Value ? null : row["CategoryCode"].ToString(),
                    OrganizationCode = row["OrganizationCode"] == DBNull.Value ? null : row["OrganizationCode"].ToString(),
                    TypeCode = (string)row["TypeCode"],
                    PrivilegeState = row["PrivilegeState"] == DBNull.Value ? null : (int?)row["PrivilegeState"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Menu> GetAllMenus()
        {
            var list = new List<Menu>();
            foreach (DataRow row in this["Menu"].Rows)
            {
                var item = new Menu
                {
                    Id = (Guid)row["Id"],
                    Name = (string)row["Name"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    AllowDelete = (int)row["AllowDelete"],
                    AllowEdit = (int)row["AllowEdit"],
                    AppSystemID = (Guid)row["AppSystemID"],
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    Url = row["Url"] == DBNull.Value ? null : row["Url"].ToString(),
                    ParentID = row["ParentID"] == DBNull.Value ? null : (Guid?)row["ParentID"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Page> GetAllPages()
        {
            var list = new List<Page>();
            foreach (DataRow row in this["Page"].Rows)
            {
                var item = new Page
                {
                    Id = (Guid)row["Id"],
                    RowVersionID = (byte[])row["RowVersionID"],
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    Tooltip = row["Tooltip"] == DBNull.Value ? null : row["Tooltip"].ToString()
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<PageButton> GetAllPageButtons()
        {
            var list = new List<PageButton>();
            foreach (DataRow row in this["PageButton"].Rows)
            {
                var item = new PageButton
                {
                    Id = (Guid)row["Id"],
                    ButtonID = (Guid)row["ButtonID"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    FunctionID = row["FunctionID"] == DBNull.Value ? null : (Guid?)row["FunctionID"],
                    IsEnabled = (int)row["IsEnabled"],
                    PageID = (Guid)row["PageID"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<PrivilegeBigram> GetPrivilegeBigrams()
        {
            var list = new List<PrivilegeBigram>();
            foreach (DataRow row in this["PrivilegeBigram"].Rows)
            {
                var item = new PrivilegeBigram
                {
                    Id = (Guid)row["Id"],
                    SubjectType = (string)row["SubjectType"],
                    SubjectInstanceID = (Guid)row["SubjectInstanceID"],
                    ObjectType = (string)row["ObjectType"],
                    ObjectInstanceID = (Guid)row["ObjectInstanceID"],
                    PrivilegeOrientation = (int)row["PrivilegeOrientation"],
                    PrivilegeConstraint = row["PrivilegeConstraint"] == DBNull.Value ? null : row["PrivilegeConstraint"].ToString(),
                    RowVersionID = (byte[])row["RowVersionID"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<ResourceType> GetAllResources()
        {
            var list = new List<ResourceType>();
            foreach (DataRow row in this["ResourceType"].Rows)
            {
                var item = new ResourceType
                {
                    Id = (Guid)row["Id"],
                    AllowDelete = (int)row["AllowDelete"],
                    AllowEdit = (int)row["AllowEdit"],
                    Code = (string)row["Code"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    Name = row["Name"] == DBNull.Value ? null : row["Name"].ToString(),
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    AppSystemID = (Guid)row["AppSystemID"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Role> GetAllRoles()
        {
            var list = new List<Role>();
            foreach (DataRow row in this["Role"].Rows)
            {
                var item = new Role
                {
                    Id = (Guid)row["Id"],
                    AllowDelete = (int)row["AllowDelete"],
                    AllowEdit = (int)row["AllowEdit"],
                    Description = row["Description"] == DBNull.Value ? null : row["Description"].ToString(),
                    Icon = row["Icon"] == DBNull.Value ? null : row["Icon"].ToString(),
                    Name = row["Name"] == DBNull.Value ? null : row["Name"].ToString(),
                    RowVersionID = (byte[])row["RowVersionID"],
                    SortCode = (int)row["SortCode"],
                    CategoryCode = row["CategoryCode"] == DBNull.Value ? null : row["CategoryCode"].ToString(),
                    IsEnabled = (int)row["IsEnabled"],
                    NumberID = (int)row["NumberID"],
                    PrivilegeState = row["PrivilegeState"] == DBNull.Value ? null : (int?)row["PrivilegeState"]
                };
                var entity = item as IEntityBase;
                entity.CreateBy = row["CreateBy"] == DBNull.Value ? null : row["CreateBy"].ToString();
                entity.CreateOn = row["CreateOn"] == DBNull.Value ? null : (DateTime?)row["CreateOn"];
                entity.CreateUserID = row["CreateUserID"] == DBNull.Value ? null : (Guid?)row["CreateUserID"];
                entity.ModifiedBy = row["ModifiedBy"] == DBNull.Value ? null : row["ModifiedBy"].ToString();
                entity.ModifiedOn = row["ModifiedOn"] == DBNull.Value ? null : (DateTime?)row["ModifiedOn"];
                entity.ModifiedUserID = row["ModifiedUserID"] == DBNull.Value ? null : (Guid?)row["ModifiedUserID"];

                list.Add(item);
            }
            return list;
        }

        public IList<Account> GetAllDevAccounts()
        {
            var list = new List<Account>();
            PropertyInfo[] properties = typeof(Account).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            DataColumn[] columns = new DataColumn[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                DataColumn column = null;
                foreach (DataColumn item in this["Account"].Columns)
                {
                    if (item.ColumnName.Equals(property.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        column = item;
                        break;
                    }
                }
                if (column != null)
                {
                    columns[i] = column;
                }
            }
            foreach (DataRow row in this["Account"].Rows)
            {
                var obj = new Account();
                for (int i = 0; i < columns.Length; i++)
                {
                    var column = columns[i];
                    if (column == null)
                    {
                        continue;
                    }
                    PropertyInfo property = properties[i];
                    if (!property.CanWrite)
                    {
                        continue;
                    }
                    object value = row[column];
                    if (value == DBNull.Value) value = null;

                    property.SetValue(obj, value, null);
                }
                list.Add(obj);
            }
            return list;
        }
    }
}


namespace Anycmd.Host
{
    using AC;
    using AC.Identity;
    using AC.Infra;
    using Anycmd.AC;
    using Anycmd.Rdb;
    using Repositories;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using Util;

    public class DefaultAppHostBootstrap : IAppHostBootstrap
    {
        /// <summary>
        /// 数据库连接字符串引导库连接字符串
        /// </summary>
        private string _bootConnString = ConfigurationManager.AppSettings["BootDbConnString"];

        /// <summary>
        /// 数据库连接字符串引导库连接字符串
        /// </summary>
        public string BootConnString { get { return _bootConnString; } }

        private readonly IAppHost host;

        public DefaultAppHostBootstrap(IAppHost host)
        {
            this.host = host;
        }
        public List<IParameter> GetParameters()
        {
            var list = new List<IParameter>();
            var sql = "select * from [Parameter]";
            using (var conn = new SqlConnection(this.BootConnString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = System.Data.CommandType.Text;
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new Parameter(reader));
                }
            }

            return list;
        }

        public IList<RDatabase> GetAllRDatabases()
        {
            return host.GetRequiredService<IRdbMetaDataService>().GetDatabases();
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
            var repository = host.GetRequiredService<IRepository<Organization>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<AppSystem> GetAllAppSystems()
        {
            var repository = host.GetRequiredService<IRepository<AppSystem>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Button> GetAllButtons()
        {
            var repository = host.GetRequiredService<IRepository<Button>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Dic> GetAllDics()
        {
            var repository = host.GetRequiredService<IRepository<Dic>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<DicItem> GetAllDicItems()
        {
            var repository = host.GetRequiredService<IRepository<DicItem>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<EntityType> GetAllEntityTypes()
        {
            var repository = host.GetRequiredService<IRepository<EntityType>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Property> GetAllProperties()
        {
            var repository = host.GetRequiredService<IRepository<Property>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Function> GetAllFunctions()
        {
            var repository = host.GetRequiredService<IRepository<Function>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Group> GetAllGroups()
        {
            var repository = host.GetRequiredService<IRepository<Group>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Menu> GetAllMenus()
        {
            var repository = host.GetRequiredService<IRepository<Menu>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Page> GetAllPages()
        {
            var repository = host.GetRequiredService<IRepository<Page>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<PageButton> GetAllPageButtons()
        {
            var repository = host.GetRequiredService<IRepository<PageButton>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<PrivilegeBigram> GetPrivilegeBigrams()
        {
            var repository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
            using (var context = repository.Context)
            {
                var subjectType = ACSubjectType.Account.ToName();
                return repository.FindAll().Where(a=>a.SubjectType != subjectType).ToList();
            }
        }

        public IList<ResourceType> GetAllResources()
        {
            var repository = host.GetRequiredService<IRepository<ResourceType>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Role> GetAllRoles()
        {
            var repository = host.GetRequiredService<IRepository<Role>>();
            using (var context = repository.Context)
            {
                return repository.FindAll().ToList();
            }
        }

        public IList<Account> GetAllDevAccounts()
        {
            var repository = host.GetRequiredService<IRepository<Account>>();
            using (var context = repository.Context)
            {
                return repository.Context.Query<DeveloperID>().Join(repository.Context.Query<Account>(), d => d.Id, a => a.Id, (d, a) => a).ToList();
            }
        }
    }
}

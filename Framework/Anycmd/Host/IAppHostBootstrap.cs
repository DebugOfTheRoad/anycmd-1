
namespace Anycmd.Host
{
    using AC;
    using AC.Identity;
    using AC.Infra;
    using Anycmd.Rdb;
    using System.Collections.Generic;

    public interface IAppHostBootstrap
    {
        List<IParameter> GetParameters();
        IList<RDatabase> GetAllRDatabases();
        IList<DbTableColumn> GetTableColumns(RdbDescriptor db);
        IList<DbTable> GetDbTables(RdbDescriptor db);
        IList<DbViewColumn> GetViewColumns(RdbDescriptor db);
        IList<DbView> GetDbViews(RdbDescriptor db);
        IList<Organization> GetOrganizations();
        IList<AppSystem> GetAllAppSystems();
        IList<Button> GetAllButtons();
        IList<Dic> GetAllDics();
        IList<DicItem> GetAllDicItems();
        IList<EntityType> GetAllEntityTypes();
        IList<Property> GetAllProperties();
        IList<Function> GetAllFunctions();
        IList<Group> GetAllGroups();
        IList<Menu> GetAllMenus();
        IList<Page> GetAllPages();
        IList<PageButton> GetAllPageButtons();
        IList<PrivilegeBigram> GetPrivilegeBigrams();
        IList<ResourceType> GetAllResources();
        IList<Role> GetAllRoles();
        IList<Account> GetAllDevAccounts();
    }
}

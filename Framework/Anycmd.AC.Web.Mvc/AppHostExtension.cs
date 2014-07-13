
namespace Anycmd.AC.Web.Mvc
{
    using Anycmd.Host;
    using Exceptions;
    using Infra.ViewModels.AppSystemViewModels;
    using Infra.ViewModels.ButtonViewModels;
    using Infra.ViewModels.DicViewModels;
    using Infra.ViewModels.EntityTypeViewModels;
    using Infra.ViewModels.FunctionViewModels;
    using Infra.ViewModels.PageViewModels;
    using Infra.ViewModels.ResourceViewModels;
    using Rdb;
    using RdbViewModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Util;
    using ViewModel;
    using ViewModels.GroupViewModels;
    using ViewModels.PrivilegeViewModels;
    using ViewModels.RoleViewModels;

    public static class AppHostExtension
    {
        #region GetPlistAppSystems
        public static IQueryable<AppSystemTr> GetPlistAppSystems(this AppHost host, GetPlistResult requestData)
        {
            EntityTypeState appSystemEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "AppSystem", out appSystemEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestData.filters)
            {
                PropertyState property;
                if (!appSystemEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的AppSystem实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = host.AppSystemSet.Select(a => AppSystemTr.Create(host, a)).AsQueryable();
            foreach (var filter in requestData.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistButtons
        public static IQueryable<ButtonTr> GetPlistButtons(this AppHost host, GetPlistResult requestData)
        {
            EntityTypeState buttonEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "Button", out buttonEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestData.filters)
            {
                PropertyState property;
                if (!buttonEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Button实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = host.ButtonSet.Select(a => ButtonTr.Create(a)).AsQueryable();
            foreach (var filter in requestData.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistPageButtons
        public static IQueryable<PageAssignButtonTr> GetPlistPageButtons(this AppHost host, GetPlistPageButtons requestData)
        {
            if (!requestData.pageID.HasValue)
            {
                throw new ValidationException("pageID是必须的");
            }
            PageState page;
            if (!host.PageSet.TryGetPage(requestData.pageID.Value, out page))
            {
                throw new ValidationException("意外的页面标识" + requestData.pageID);
            }
            var pageButtons = host.PageSet.GetPageButtons(page);
            var buttons = host.ButtonSet;
            var data = new List<PageAssignButtonTr>();
            foreach (var button in buttons)
            {
                var pageButton = pageButtons.FirstOrDefault(a => a.ButtonID == button.Id && a.PageID == page.Id);
                if (requestData.isAssigned.HasValue)
                {
                    if (requestData.isAssigned.Value)
                    {
                        if (pageButton == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (pageButton != null)
                        {
                            continue;
                        }
                    }
                }
                DateTime? createOn = null;
                int isEnabled;
                string functionCode = null;
                Guid? functionID = null;
                string functionName = null;
                Guid id;
                bool isAssigned;
                int functionIsEnabled = 0;
                if (pageButton != null)
                {
                    createOn = pageButton.CreateOn;
                    isEnabled = pageButton.IsEnabled;
                    id = pageButton.Id;
                    isAssigned = true;
                    FunctionState function;
                    if (pageButton.FunctionID.HasValue && host.FunctionSet.TryGetFunction(pageButton.FunctionID.Value, out function))
                    {
                        functionCode = function.Code;
                        functionID = function.Id;
                        functionName = function.Description;
                        functionIsEnabled = function.IsEnabled;
                    }
                }
                else
                {
                    id = Guid.NewGuid();
                    isAssigned = false;
                    isEnabled = 0;
                }
                data.Add(new PageAssignButtonTr
                {
                    CreateOn = createOn,
                    Id = id,
                    IsAssigned = isAssigned,
                    FunctionIsEnabled = functionIsEnabled,
                    Icon = button.Icon,
                    IsEnabled = isEnabled,
                    Name = button.Name,
                    ButtonID = button.Id,
                    ButtonIsEnabled = button.IsEnabled,
                    Code = button.Code,
                    FunctionCode = functionCode,
                    FunctionID = functionID,
                    FunctionName = functionName,
                    PageID = page.Id,
                    SortCode = button.SortCode
                });
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(requestData.key))
            {
                queryable = queryable.Where(a => a.Name.Contains(requestData.key) || a.Code.Contains(requestData.key));
            }
            requestData.total = queryable.Count();
            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistDatabases
        public static IQueryable<IRDatabase> GetPlistDatabases(this AppHost host, GetPlistResult requestModel)
        {
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "RDatabase", out entityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!entityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的RDatabase实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.Rdbs.Select(a => a.Database).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistTables
        public static IQueryable<DbTable> GetPlistTables(this AppHost host, GetPlistTables requestModel)
        {
            RdbDescriptor db;
            if (!host.Rdbs.TryDb(requestModel.databaseID.Value, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            HashSet<string> properties = new HashSet<string>()
            {
                "Id",
                "DatabaseID",
                "CatalogName",
                "SchemaName",
                "Name",
                "Description"
            };
            foreach (var filter in requestModel.filters)
            {
                if (!properties.Contains(filter.field))
                {
                    throw new ValidationException("意外的DbTable实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.DbTables[db].Values.Select(a => a).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistViews
        public static IQueryable<DbView> GetPlistViews(this AppHost host, GetPlistViews requestModel)
        {
            RdbDescriptor db;
            if (!host.Rdbs.TryDb(requestModel.databaseID.Value, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            HashSet<string> properties = new HashSet<string>()
            {
                "Id",
                "DatabaseID",
                "CatalogName",
                "SchemaName",
                "Name",
                "Description"
            };
            foreach (var filter in requestModel.filters)
            {
                if (!properties.Contains(filter.field))
                {
                    throw new ValidationException("意外的DbView实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.DbViews[db].Values.Select(a => a).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistTableColumns
        public static IQueryable<DbTableColumn> GetPlistTableColumns(this AppHost host, GetPlistTableColumns requestModel)
        {
            RdbDescriptor db;
            if (!host.Rdbs.TryDb(requestModel.databaseID.Value, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            DbTable dbTable;
            if (!host.DbTables.TryGetDbTable(db, requestModel.tableID, out dbTable))
            {
                throw new ValidationException("意外的数据库表名" + requestModel.tableName);
            }
            HashSet<string> properties = new HashSet<string>()
            {
                "DatabaseID",
                "Id",
                "CatalogName",
                "DateTimePrecision",
                "DefaultValue",
                "Description",
                "IsIdentity",
                "IsNullable",
                "IsPrimaryKey",
                "IsStoreGenerated",
                "MaxLength",
                "Name",
                "Ordinal",
                "Precision",
                "Scale",
                "SchemaName",
                "TableName",
                "TypeName"
            };
            foreach (var filter in requestModel.filters)
            {
                if (!properties.Contains(filter.field))
                {
                    throw new ValidationException("意外的DbTableColumn实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            IReadOnlyDictionary<string, DbTableColumn> dbTableColumns;
            if (!host.DbTableColumns.TryGetDbTableColumns(db, dbTable, out dbTableColumns))
            {
                throw new CoreException("意外的数据库表列");
            }
            var queryable = dbTableColumns.Values.Select(a => a).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistViewColumns
        public static IQueryable<DbViewColumn> GetPlistViewColumns(this AppHost host, GetPlistViewColumns requestModel)
        {
            RdbDescriptor db;
            if (!host.Rdbs.TryDb(requestModel.databaseID.Value, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            DbView dbView;
            if (!host.DbViews.TryGetDbView(db, requestModel.viewID, out dbView))
            {
                throw new ValidationException("意外的数据库表名" + requestModel.viewName);
            } HashSet<string> properties = new HashSet<string>()
            {
                "DatabaseID",
                "Id",
                "CatalogName",
                "DateTimePrecision",
                "DefaultValue",
                "Description",
                "IsIdentity",
                "IsNullable",
                "IsPrimaryKey",
                "IsStoreGenerated",
                "MaxLength",
                "Name",
                "Ordinal",
                "Precision",
                "Scale",
                "SchemaName",
                "TableName",
                "TypeName"
            };
            foreach (var filter in requestModel.filters)
            {
                if (!properties.Contains(filter.field))
                {
                    throw new ValidationException("意外的DbViewColumn实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            IReadOnlyDictionary<string, DbViewColumn> dbViewColumns;
            if (!host.DbViewColumns.TryGetDbViewColumns(db, dbView, out dbViewColumns))
            {
                throw new CoreException("意外的数据库视图列");
            }
            var queryable = dbViewColumns.Values.Select(a => a).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistDics
        public static IQueryable<DicTr> GetPlistDics(this AppHost host, GetPlistResult requestModel)
        {
            EntityTypeState dicEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "Dic", out dicEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!dicEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Dic实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.DicSet.Select(a => DicTr.Create(a)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistDicItems
        public static IQueryable<DicItemTr> GetPlistDicItems(this AppHost host, GetPlistDicItems requestModel)
        {
            if (!requestModel.dicID.HasValue)
            {
                throw new ValidationException("字典标识是必须的");
            }
            if (!host.DicSet.ContainsDic(requestModel.dicID.Value))
            {
                throw new ValidationException("意外的系统字典标识" + requestModel.dicID);
            }
            EntityTypeState dicItemEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "DicItem", out dicItemEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!dicItemEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的DicItem实体类型属性" + filter.field);
                }
            }
            DicState dic;
            if (!host.DicSet.TryGetDic(requestModel.dicID.Value, out dic))
            {
                throw new ValidationException("意外的字典标识" + requestModel.dicID.Value);
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.DicSet.GetDicItems(dic).Select(a => DicItemTr.Create(a.Value)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            if (!string.IsNullOrEmpty(requestModel.key))
            {
                queryable = queryable.Where(a => a.Code.ToLower().Contains(requestModel.key) || a.Name.ToLower().Contains(requestModel.key));
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistEntityTypes
        public static IQueryable<EntityTypeTr> GetPlistEntityTypes(this AppHost host, GetPlistResult requestData)
        {
            EntityTypeState entityTypeEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "EntityType", out entityTypeEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestData.filters)
            {
                PropertyState property;
                if (!host.EntityTypeSet.TryGetProperty(entityTypeEntityType, filter.field, out property))
                {
                    throw new ValidationException("意外的EntityType实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = host.EntityTypeSet.Select(a => EntityTypeTr.Create(a)).AsQueryable();
            foreach (var filter in requestData.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistFunctions
        public static IQueryable<FunctionTr> GetPlistFunctions(this AppHost host, GetPlistResult requestData)
        {
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "Function", out entityType))
            {
                throw new CoreException("意外的实体类型AC.Function");
            }
            foreach (var filter in requestData.filters)
            {
                PropertyState property;
                if (!host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Function实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = host.FunctionSet.Select(a => FunctionTr.Create(a)).AsQueryable();
            foreach (var filter in requestData.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistPrivilegeByRoleID
        public static IQueryable<RoleAssignFunctionTr> GetPlistPrivilegeByRoleID(this AppHost host, GetPlistFunctionByRoleID requestData)
        {
            AppSystemState appSystem;
            if (!host.AppSystemSet.TryGetAppSystem(requestData.appSystemID, out appSystem))
            {
                throw new ValidationException("意外的应用系统标识" + requestData.appSystemID);
            }
            RoleState role;
            if (!host.RoleSet.TryGetRole(requestData.roleID, out role))
            {
                throw new ValidationException("意外的角色标识" + requestData.roleID);
            }
            if (requestData.resourceTypeID.HasValue)
            {
                ResourceTypeState resource;
                if (!host.ResourceSet.TryGetResource(requestData.resourceTypeID.Value, out resource))
                {
                    throw new ValidationException("意外的资源标识" + requestData.resourceTypeID);
                }
            }
            var roleFunctions = host.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Function);
            var functions = host.FunctionSet.Where(a => a.AppSystem.Id == requestData.appSystemID && a.IsManaged);
            if (requestData.resourceTypeID.HasValue)
            {
                functions = functions.Where(a => a.ResourceTypeID == requestData.resourceTypeID.Value);
            }
            var data = new List<RoleAssignFunctionTr>();
            foreach (var function in functions)
            {
                var roleFunction = roleFunctions.FirstOrDefault(a => a.SubjectInstanceID == role.Id && a.ObjectInstanceID == function.Id);
                if (requestData.isAssigned.HasValue)
                {
                    if (requestData.isAssigned.Value)
                    {
                        if (roleFunction == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (roleFunction != null)
                        {
                            continue;
                        }
                    }
                }
                string createBy = null;
                DateTime? createOn = null;
                Guid? createUserID = null;
                Guid id;
                bool isAssigned;
                int orientation = 0;
                string privilegeConstraint = null;
                if (roleFunction != null)
                {
                    createBy = roleFunction.CreateBy;
                    createOn = roleFunction.CreateOn;
                    createUserID = roleFunction.CreateUserID;
                    id = roleFunction.Id;
                    isAssigned = true;
                    orientation = roleFunction.PrivilegeOrientation;
                    privilegeConstraint = roleFunction.PrivilegeConstraint;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAssigned = false;
                }
                data.Add(new RoleAssignFunctionTr
                {
                    AppSystemCode = appSystem.Code,
                    AppSystemID = appSystem.Id,
                    CreateBy = createBy,
                    CreateOn = createOn,
                    CreateUserID = createUserID,
                    Description = function.Description,
                    FunctionCode = function.Code,
                    FunctionID = function.Id,
                    Id = id,
                    IsAssigned = isAssigned,
                    PrivilegeOrientation = orientation,
                    PrivilegeConstraint = privilegeConstraint,
                    ResourceCode = function.Resource.Code,
                    ResourceTypeID = function.ResourceTypeID,
                    ResourceName = function.Resource.Name,
                    RoleID = requestData.roleID,
                    SortCode = function.SortCode
                });
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(requestData.key))
            {
                queryable = queryable.Where(a => a.Description.Contains(requestData.key) || a.FunctionCode.Contains(requestData.key));
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistGroups
        public static IQueryable<GroupTr> GetPlistGroups(this AppHost host, GetPlistResult requestModel)
        {
            EntityTypeState groupEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "Group", out groupEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!groupEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Group实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.GroupSet.Select(a => GroupTr.Create(a)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistRoleGroups
        public static IQueryable<RoleAssignGroupTr> GetPlistRoleGroups(this AppHost host, GetPlistRoleGroups requestData)
        {
            RoleState role;
            if (!host.RoleSet.TryGetRole(requestData.roleID, out role))
            {
                throw new ValidationException("意外的角色标识" + requestData.roleID);
            }
            List<RoleAssignGroupTr> data = new List<RoleAssignGroupTr>();
            foreach (var group in host.GroupSet)
            {
                var roleGroup = host.PrivilegeSet.FirstOrDefault(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Group && a.SubjectInstanceID == role.Id && a.ObjectInstanceID == group.Id);
                if (requestData.isAssigned.HasValue)
                {
                    if (requestData.isAssigned.Value)
                    {
                        if (roleGroup == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (roleGroup != null)
                        {
                            continue;
                        }
                    }
                }
                string createBy = null;
                DateTime? createOn = null;
                Guid? createUserID = null;
                Guid id;
                bool isAssigned;
                if (roleGroup != null)
                {
                    createBy = roleGroup.CreateBy;
                    createOn = roleGroup.CreateOn;
                    createUserID = roleGroup.CreateUserID;
                    id = roleGroup.Id;
                    isAssigned = true;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAssigned = false;
                }
                data.Add(new RoleAssignGroupTr
                {
                    CategoryCode = group.CategoryCode,
                    CreateBy = createBy,
                    CreateOn = createOn,
                    CreateUserID = createUserID,
                    GroupID = group.Id,
                    Id = id,
                    IsAssigned = isAssigned,
                    IsEnabled = group.IsEnabled,
                    Name = group.Name,
                    RoleID = role.Id,
                    SortCode = group.SortCode
                });
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(requestData.key))
            {
                queryable = queryable.Where(a => a.Name.Contains(requestData.key) || a.CategoryCode.Contains(requestData.key));
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistPages
        public static IQueryable<PageTr> GetPlistPages(this AppHost host, GetPlistResult requestData)
        {
            EntityTypeState pageEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "Page", out pageEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestData.filters)
            {
                PropertyState property;
                if (!pageEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Page实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = host.PageSet.Select(a => PageTr.Create(a)).AsQueryable();
            foreach (var filter in requestData.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistProperties
        public static IQueryable<PropertyTr> GetPlistProperties(this AppHost host, GetPlistProperties requestModel)
        {
            if (!requestModel.entityTypeID.HasValue)
            {
                throw new ValidationException("entityTypeID是必须的");
            }
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "Property", out entityType))
            {
                throw new CoreException("意外的实体类型AC.Property");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!host.EntityTypeSet.TryGetProperty(entityType, filter.field, out property))
                {
                    throw new ValidationException("意外的Property实体类型属性" + filter.field);
                }
            }
            if (!host.EntityTypeSet.TryGetEntityType(requestModel.entityTypeID.Value, out entityType))
            {
                throw new CoreException("意外的实体类型标识" + requestModel.entityTypeID.Value);
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.EntityTypeSet.GetProperties(entityType).Select(a => PropertyTr.Create(a.Value)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            if (!string.IsNullOrEmpty(requestModel.key))
            {
                queryable = queryable.Where(a => a.Code.ToLower().Contains(requestModel.key) || a.Name.ToLower().Contains(requestModel.key));
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistResources
        public static IQueryable<ResourceTypeTr> GetPlistResources(this AppHost host, GetPlistResult requestModel)
        {
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "ResourceType", out entityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!entityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Resource实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.ResourceSet.Select(a => ResourceTypeTr.Create(a)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistAppSystemResources
        public static IQueryable<ResourceTypeTr> GetPlistAppSystemResources(this AppHost host, GetPlistAreaResourceTypes requestModel)
        {
            if (!host.AppSystemSet.ContainsAppSystem(requestModel.appSystemID))
            {
                throw new CoreException("意外的应用系统标识" + requestModel.appSystemID);
            }
            IEnumerable<Guid> resourceTypeIDs = host.FunctionSet.Where(a => a.AppSystem.Id == requestModel.appSystemID).Select(a => a.ResourceTypeID).Distinct();
            List<ResourceTypeState> resources = new List<ResourceTypeState>();
            foreach (var resourceTypeID in resourceTypeIDs)
            {
                ResourceTypeState resource;
                host.ResourceSet.TryGetResource(resourceTypeID, out resource);
                resources.Add(resource);
            }
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "ResourceType", out entityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!entityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Resource实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = resources.Select(a => ResourceTypeTr.Create(a)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            if (!string.IsNullOrEmpty(requestModel.key))
            {
                queryable = queryable.Where(a => a.Code.ToLower().Contains(requestModel.key) || a.Name.ToLower().Contains(requestModel.key));
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistRoles
        public static IQueryable<RoleTr> GetPlistRoles(this AppHost host, GetPlistResult requestModel)
        {
            EntityTypeState roleEntityType;
            if (!host.EntityTypeSet.TryGetEntityType("AC", "Role", out roleEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            foreach (var filter in requestModel.filters)
            {
                PropertyState property;
                if (!roleEntityType.TryGetProperty(filter.field, out property))
                {
                    throw new ValidationException("意外的Role实体类型属性" + filter.field);
                }
            }
            int pageIndex = requestModel.pageIndex ?? 0;
            int pageSize = requestModel.pageSize ?? 10;
            var queryable = host.RoleSet.Select(a => RoleTr.Create(a)).AsQueryable();
            foreach (var filter in requestModel.filters)
            {
                queryable = queryable.Where(filter.ToPredicate(), filter.value);
            }
            requestModel.total = queryable.Count();

            return queryable.OrderBy(requestModel.sortField + " " + requestModel.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistGroupRoles
        public static IQueryable<GroupAssignRoleTr> GetPlistGroupRoles(this AppHost host, GetPlistGroupRoles requestData)
        {
            GroupState group;
            if (!host.GroupSet.TryGetGroup(requestData.groupID, out group))
            {
                throw new ValidationException("意外的工作组标识" + requestData.groupID);
            }
            List<GroupAssignRoleTr> data = new List<GroupAssignRoleTr>();
            foreach (var role in host.RoleSet)
            {
                var roleGroup = host.PrivilegeSet.FirstOrDefault(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Group && a.SubjectInstanceID == role.Id && a.ObjectInstanceID == group.Id);
                if (requestData.isAssigned.HasValue)
                {
                    if (requestData.isAssigned.Value)
                    {
                        if (roleGroup == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (roleGroup != null)
                        {
                            continue;
                        }
                    }
                }
                string createBy = null;
                DateTime? createOn = null;
                Guid? createUserID = null;
                Guid id;
                bool isAssigned;
                if (roleGroup != null)
                {
                    createBy = roleGroup.CreateBy;
                    createOn = roleGroup.CreateOn;
                    createUserID = roleGroup.CreateUserID;
                    id = roleGroup.Id;
                    isAssigned = true;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAssigned = false;
                }
                data.Add(new GroupAssignRoleTr
                {
                    CategoryCode = role.CategoryCode,
                    CreateBy = createBy,
                    CreateOn = createOn,
                    CreateUserID = createUserID,
                    GroupID = group.Id,
                    Id = id,
                    IsAssigned = isAssigned,
                    IsEnabled = role.IsEnabled,
                    Name = role.Name,
                    RoleID = role.Id,
                    SortCode = role.SortCode,
                    Icon = role.Icon
                });
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(requestData.key))
            {
                queryable = queryable.Where(a => a.Name.Contains(requestData.key) || a.CategoryCode.Contains(requestData.key));
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion

        #region GetPlistMenuRoles
        public static IQueryable<MenuAssignRoleTr> GetPlistMenuRoles(this AppHost host, GetPlistMenuRoles requestData)
        {
            MenuState menu;
            if (!host.MenuSet.TryGetMenu(requestData.menuID.Value, out menu))
            {
                throw new ValidationException("意外的菜单标识" + requestData.menuID);
            }
            var roleMenus = host.PrivilegeSet.Where(a => a.SubjectType == ACSubjectType.Role && a.ObjectType == ACObjectType.Menu);
            var roles = host.RoleSet;
            var data = new List<MenuAssignRoleTr>();
            foreach (var role in roles)
            {
                var roleMenu = roleMenus.FirstOrDefault(a => a.SubjectInstanceID == role.Id && a.ObjectInstanceID == menu.Id);
                if (requestData.isAssigned.HasValue)
                {
                    if (requestData.isAssigned.Value)
                    {
                        if (roleMenu == null)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (roleMenu != null)
                        {
                            continue;
                        }
                    }
                }
                string createBy = null;
                DateTime? createOn = null;
                Guid? createUserID = null;
                Guid id;
                bool isAssigned;
                int orientation = 0;
                string privilegeConstraint = null;
                if (roleMenu != null)
                {
                    createBy = roleMenu.CreateBy;
                    createOn = roleMenu.CreateOn;
                    createUserID = roleMenu.CreateUserID;
                    id = roleMenu.Id;
                    isAssigned = true;
                    orientation = roleMenu.PrivilegeOrientation;
                    privilegeConstraint = roleMenu.PrivilegeConstraint;
                }
                else
                {
                    id = Guid.NewGuid();
                    isAssigned = false;
                }
                data.Add(new MenuAssignRoleTr
                {
                    CreateBy = createBy,
                    CreateOn = createOn,
                    CreateUserID = createUserID,
                    Id = id,
                    IsAssigned = isAssigned,
                    PrivilegeOrientation = orientation,
                    PrivilegeConstraint = privilegeConstraint,
                    RoleID = role.Id,
                    CategoryCode = role.CategoryCode,
                    Icon = role.Icon,
                    IsEnabled = role.IsEnabled,
                    MenuID = requestData.menuID.Value,
                    Name = role.Name,
                    SortCode = role.SortCode
                });
            }
            int pageIndex = requestData.pageIndex ?? 0;
            int pageSize = requestData.pageSize ?? 10;
            var queryable = data.AsQueryable();
            if (!string.IsNullOrEmpty(requestData.key))
            {
                queryable = queryable.Where(a => a.Name.Contains(requestData.key) || a.CategoryCode.Contains(requestData.key));
            }
            requestData.total = queryable.Count();

            return queryable.OrderBy(requestData.sortField + " " + requestData.sortOrder).Skip(pageIndex * pageSize).Take(pageSize);
        }
        #endregion
    }
}

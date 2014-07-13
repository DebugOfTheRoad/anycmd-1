
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using MiniUI;
    using Rdb;
    using RdbViewModel;
    using System;
    using System.ComponentModel;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 数据库模型视图控制器<see cref="Anycmd.Rdb.RDatabase"/>
    /// </summary>
    public class RDatabaseController : AnycmdController
    {
        #region ViewResults
        [By("xuexs")]
        [Description("数据库模块主页")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("数据库文档列表页")]
        public ViewResultBase DbDocs()
        {
            return View();
        }

        [By("xuexs")]
        [Description("数据库文档详细页")]
        public ViewResultBase DbDoc(Guid databaseID)
        {
            RdbDescriptor rdb;
            if (!AppHostInstance.Rdbs.TryDb(databaseID, out rdb))
            {
                throw new ValidationException("意外的关系数据库标识" + databaseID);
            }
            return View(rdb);
        }

        [By("xuexs")]
        [Description("数据库表")]
        public ViewResultBase Tables()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("数据库视图")]
        public ViewResultBase Views()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("视图定义")]
        public ViewResultBase ViewDefinition()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("表空间")]
        public ViewResultBase TableSpaces()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("数据库表列")]
        public ViewResultBase TableColumns()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("数据库视图列")]
        public ViewResultBase ViewColumns()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("数据库详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                if (!string.IsNullOrEmpty(Request["id"]))
                {
                    Guid id;
                    if (Guid.TryParse(Request["id"], out id))
                    {
                        var data = GetRequiredService<IRdbMetaDataService>().GetDatabase(id);
                        return new PartialViewResult
                        {
                            ViewName = "Partials/Details",
                            ViewData = new ViewDataDictionary(DatabaseInfo.Create(data))
                        };
                    }
                }
                throw new ValidationException("非法的Guid标识" + Request["id"]);
            }
            else if (!string.IsNullOrEmpty(Request["isInner"]))
            {
                return new PartialViewResult { ViewName = "Partials/Details" };
            }
            else
            {
                return new ViewResult { ViewName = "Details" };
            }
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取数据库")]
        public ActionResult Get(Guid? id)
        {
            RDatabase data = null;
            if (id.HasValue)
            {
                data = GetRequiredService<IRdbMetaDataService>().GetDatabase(id.Value);
            }

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("根据ID获取数据库详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            DatabaseInfo data = null;
            if (id.HasValue)
            {
                data = DatabaseInfo.Create(GetRequiredService<IRdbMetaDataService>().GetDatabase(id.Value));
            }

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("根据ID获取数据库表文档")]
        public ActionResult GetTable(Guid databaseID, string id)
        {
            RdbDescriptor db;
            if (!AppHostInstance.Rdbs.TryDb(databaseID, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            DbTable dbTable;
            if (!AppHostInstance.DbTables.TryGetDbTable(db, id, out dbTable))
            {
                throw new ValidationException("意外的数据库表标识" + id);
            }

            return this.JsonResult(dbTable);
        }

        [By("xuexs")]
        [Description("根据ID获取数据库视图文档")]
        public ActionResult GetView(Guid databaseID, string id)
        {
            RdbDescriptor db;
            if (!AppHostInstance.Rdbs.TryDb(databaseID, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            DbView dbView;
            if (!AppHostInstance.DbViews.TryGetDbView(db, id, out dbView))
            {
                throw new ValidationException("意外的数据库视图标识" + id);
            }
            return this.JsonResult(dbView);
        }

        [By("xuexs")]
        [Description("获取视图定义")]
        public ActionResult GetViewDefinition(Guid databaseID, string viewID)
        {
            RdbDescriptor db;
            if (!AppHostInstance.Rdbs.TryDb(databaseID, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            DbView view;
            if (!AppHostInstance.DbViews.TryGetDbView(db, viewID, out view))
            {
                throw new ValidationException("意外的数据库视图" + viewID);
            }
            return this.Content(GetRequiredService<IRdbMetaDataService>().GetViewDefinition(db, view));
        }

        [By("xuexs")]
        [Description("根据ID获取数据库表列文档")]
        public ActionResult GetTableColumn(Guid databaseID, string id)
        {
            RdbDescriptor db;
            if (!AppHostInstance.Rdbs.TryDb(databaseID, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            DbTableColumn colum;
            if (!AppHostInstance.DbTableColumns.TryGetDbTableColumn(db, id, out colum))
            {
                throw new ValidationException("意外的数据库表列标识" + id);
            }
            return this.JsonResult(colum);
        }

        [By("xuexs")]
        [Description("根据ID获取数据库视图列文档")]
        public ActionResult GetViewColumn(Guid databaseID, string id)
        {
            RdbDescriptor db;
            if (!AppHostInstance.Rdbs.TryDb(databaseID, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            DbViewColumn colum;
            if (!AppHostInstance.DbViewColumns.TryGetDbViewColumn(db, id, out colum))
            {
                throw new ValidationException("意外的数据库视图列标识" + id);
            }
            return this.JsonResult(colum);
        }

        [By("xuexs")]
        [Description("分页获取数据库")]
        public ActionResult GetPlistDatabases(GetPlistResult requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistDatabases(requestModel);

            return this.JsonResult(new MiniGrid<IRDatabase> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("分页获取数据库表")]
        public ActionResult GetPlistTables(GetPlistTables requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistTables(requestModel);

            return this.JsonResult(new MiniGrid<DbTable> { total = requestModel.total.Value, data = data });
        }


        [By("xuexs")]
        [Description("查看表空间使用情况")]
        public ActionResult GetTableSpaces(Guid? databaseID, string sortField, string sortOrder)
        {
            if (!databaseID.HasValue)
            {
                throw new ValidationException("未传入databaseID");
            }
            RdbDescriptor db;
            if (!AppHostInstance.Rdbs.TryDb(databaseID.Value, out db))
            {
                throw new ValidationException("意外的数据库ID");
            }
            var spaces = GetRequiredService<IRdbMetaDataService>().GetTableSpaces(db, sortField, sortOrder);

            return this.JsonResult(spaces);
        }

        [By("xuexs")]
        [Description("分页获取数据库表")]
        public ActionResult GetPlistViews(GetPlistViews requestModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistViews(requestModel);

            return this.JsonResult(new MiniGrid<DbView> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("分页获取数据库表列")]
        public ActionResult GetPlistTableColumns(GetPlistTableColumns requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistTableColumns(requestModel);

            return this.JsonResult(new MiniGrid<DbTableColumn> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("分页获取数据库视图列")]
        public ActionResult GetPlistViewColumns(GetPlistViewColumns requestModel)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistViewColumns(requestModel);

            return this.JsonResult(new MiniGrid<DbViewColumn> { total = requestModel.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("更新数据库信息")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(DatabaseInput input)
        {
            var responseResult = new ResponseData { success = true, id = input.Id };
            if (ModelState.IsValid)
            {
                GetRequiredService<IRdbMetaDataService>().UpdateDatabase(input.Id, input.DataSource, input.Description);
                AppHostInstance.PublishOperatedEvent(input.Id);
            }
            else
            {
                responseResult.success = false;
                string msg = string.Empty;
                foreach (var item in ModelState)
                {
                    foreach (var e in item.Value.Errors)
                    {
                        msg += e.ErrorMessage;
                    }
                }

                responseResult.msg = msg;
            }

            return this.JsonResult(responseResult);
        }

        [By("xuexs")]
        [Description("更新数据库表文档")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult UpdateTable(DbTableInput input)
        {
            var responseResult = new ResponseData { success = true, id = input.Id };
            if (ModelState.IsValid)
            {
                RdbDescriptor db;
                if (!AppHostInstance.Rdbs.TryDb(input.DatabaseID, out db))
                {
                    throw new ValidationException("意外的数据库ID");
                }
                GetRequiredService<IRdbMetaDataService>().CrudDescription(db, RDbMetaDataType.Table, input.Id, input.Description);
            }
            else
            {
                responseResult.success = false;
                string msg = string.Empty;
                foreach (var item in ModelState)
                {
                    foreach (var e in item.Value.Errors)
                    {
                        msg += e.ErrorMessage;
                    }
                }

                responseResult.msg = msg;
            }

            return this.JsonResult(responseResult);
        }

        [By("xuexs")]
        [Description("更新数据库视图文档")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult UpdateView(DbViewInput input)
        {
            var responseResult = new ResponseData { success = true, id = input.Id };
            if (ModelState.IsValid)
            {
                RdbDescriptor db;
                if (!AppHostInstance.Rdbs.TryDb(input.DatabaseID, out db))
                {
                    throw new ValidationException("意外的数据库ID");
                }
                GetRequiredService<IRdbMetaDataService>().CrudDescription(db, RDbMetaDataType.View, input.Id, input.Description);
            }
            else
            {
                responseResult.success = false;
                string msg = string.Empty;
                foreach (var item in ModelState)
                {
                    foreach (var e in item.Value.Errors)
                    {
                        msg += e.ErrorMessage;
                    }
                }

                responseResult.msg = msg;
            }

            return this.JsonResult(responseResult);
        }

        [By("xuexs")]
        [Description("更新数据库表列文档")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult UpdateTableColumn(DbTableColumnInput input)
        {
            var responseResult = new ResponseData { success = true, id = input.Id };
            if (ModelState.IsValid)
            {
                RdbDescriptor db;
                if (!AppHostInstance.Rdbs.TryDb(input.DatabaseID, out db))
                {
                    throw new ValidationException("意外的数据库ID");
                }
                GetRequiredService<IRdbMetaDataService>().CrudDescription(db, RDbMetaDataType.TableColumn, input.Id, input.Description);
            }
            else
            {
                responseResult.success = false;
                string msg = string.Empty;
                foreach (var item in ModelState)
                {
                    foreach (var e in item.Value.Errors)
                    {
                        msg += e.ErrorMessage;
                    }
                }

                responseResult.msg = msg;
            }

            return this.JsonResult(responseResult);
        }

        [By("xuexs")]
        [Description("更新数据库视图列文档")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult UpdateViewColumn(DbViewColumnInput input)
        {
            var responseResult = new ResponseData { success = true, id = input.Id };
            if (ModelState.IsValid)
            {
                RdbDescriptor db;
                if (!AppHostInstance.Rdbs.TryDb(input.DatabaseID, out db))
                {
                    throw new ValidationException("意外的数据库ID");
                }
                GetRequiredService<IRdbMetaDataService>().CrudDescription(db, RDbMetaDataType.ViewColumn, input.Id, input.Description);
            }
            else
            {
                responseResult.success = false;
                string msg = string.Empty;
                foreach (var item in ModelState)
                {
                    foreach (var e in item.Value.Errors)
                    {
                        msg += e.ErrorMessage;
                    }
                }

                responseResult.msg = msg;
            }

            return this.JsonResult(responseResult);
        }
    }
}

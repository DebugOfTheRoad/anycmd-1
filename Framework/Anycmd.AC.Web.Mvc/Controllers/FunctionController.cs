
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Infra.ViewModels.AppSystemViewModels;
    using Infra.ViewModels.FunctionViewModels;
    using MiniUI;
    using Repositories;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;
    using ViewModels.PrivilegeViewModels;

    /// <summary>
    /// 系统功能模型视图控制器<see cref="ACEntities.Function"/>
    /// </summary>
    public class FunctionController : AnycmdController
    {
        private readonly EntityTypeState functionEntityType;

        public FunctionController()
        {
            if (!AppHostInstance.EntityTypeSet.TryGetEntityType("AC", "Function", out functionEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewPages

        [By("xuexs")]
        [Description("功能集")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("功能详细信息")]
        public ViewResultBase Details()
        {
            if (!string.IsNullOrEmpty(Request["isTooltip"]))
            {
                Guid id;
                if (Guid.TryParse(Request["id"], out id))
                {
                    var data = FunctionInfo.Create(functionEntityType.GetData(id));
                    return new PartialViewResult { ViewName = "Partials/Details", ViewData = new ViewDataDictionary(data) };
                }
                else
                {
                    throw new ValidationException("非法的Guid标识" + Request["id"]);
                }
            }
            else if (!string.IsNullOrEmpty(Request["isInner"]))
            {
                return new PartialViewResult { ViewName = "Partials/Details" };
            }
            else
            {
                return this.View();
            }
        }

        #endregion

        [By("xuexs")]
        [Description("刷新功能列表")]
        [DeveloperFilter(Order = 21)]
        public ActionResult Refresh()
        {
            var result = new ResponseData { success = true };
            if (!CurrentUser.IsDeveloper())
            {
                result.success = false;
                result.msg = "对不起，您不是开发人员，不能执行本功能";
            }
            else
            {
                GetRequiredService<IFunctionListImport>().Import(AppHostInstance, AppHostInstance.Config.SelfAppSystemCode);
            }

            return this.JsonResult(result);
        }

        [By("xuexs")]
        [Description("根据ID获取功能")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(functionEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取功能详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(FunctionInfo.Create(functionEntityType.GetData(id.Value)));
        }

        [By("xuexs")]
        [Description("分页获取功能")]
        public ActionResult GetPlistFunctions(GetPlistResult requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistFunctions(requestData);

            return this.JsonResult(new MiniGrid<FunctionTr> { total = requestData.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("添加功能")]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(FunctionCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new AddFunctionCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("更新功能信息")]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(FunctionUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            AppHostInstance.Handle(new UpdateFunctionCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("删除功能")]
        [DeveloperFilter(Order = 21)]
        public ActionResult Delete(string id)
        {
            string[] ids = id.Split(',');
            var idArray = new Guid[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                Guid tmp;
                if (Guid.TryParse(ids[i], out tmp))
                {
                    idArray[i] = tmp;
                }
                else
                {
                    throw new ValidationException("意外的功能标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                AppHostInstance.Handle(new RemoveFunctionCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }

        [By("xuexs")]
        [Description("托管")]
        public ActionResult Manage(string id)
        {
            string[] ids = id.Split(',');
            var idArray = new Guid[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                Guid tmp;
                if (Guid.TryParse(ids[i], out tmp))
                {
                    idArray[i] = tmp;
                }
                else
                {
                    throw new ValidationException("意外的功能标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                var entity = GetRequiredService<IRepository<Function>>().GetByKey(item);
                var input = new FunctionUpdateInput
                {
                    Code = entity.Code,
                    Description = entity.Description,
                    DeveloperID = entity.DeveloperID,
                    Id = entity.Id,
                    IsEnabled = entity.IsEnabled,
                    IsManaged = entity.IsManaged,
                    SortCode = entity.SortCode
                };
                input.IsManaged = true;
                AppHostInstance.Handle(new UpdateFunctionCommand(input));
            }
            return this.JsonResult(new ResponseData { id = id, success = true });
        }

        [By("xuexs")]
        [Description("取消托管")]
        public ActionResult UnManage(string id)
        {
            string[] ids = id.Split(',');
            var idArray = new Guid[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                Guid tmp;
                if (Guid.TryParse(ids[i], out tmp))
                {
                    idArray[i] = tmp;
                }
                else
                {
                    throw new ValidationException("意外的功能标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                var entity = GetRequiredService<IRepository<Function>>().GetByKey(item);
                var input = new FunctionUpdateInput
                {
                    Id = entity.Id,
                    Code = entity.Code,
                    SortCode = entity.SortCode,
                    IsManaged = entity.IsManaged,
                    IsEnabled = entity.IsEnabled,
                    DeveloperID = entity.DeveloperID,
                    Description = entity.Description
                };
                input.IsManaged = false;
                AppHostInstance.Handle(new UpdateFunctionCommand(input));
            }
            return this.JsonResult(new ResponseData { id = id, success = true });
        }

        [By("xuexs")]
        [Description("获取给定应用系统给定类型资源的托管功能")]
        public ActionResult GetManagedFunctions(Guid? appSystemID, string pageController)
        {
            ResourceTypeState resource;
            if (!AppHostInstance.ResourceSet.TryGetResource(AppHostInstance.AppSystemSet.SelfAppSystem, pageController, out resource))
            {
                throw new ValidationException("意外的资源码" + pageController);
            }
            IEnumerable<FunctionTr> data = null;
            if (appSystemID.HasValue && !string.IsNullOrEmpty(pageController))
            {
                data = AppHostInstance.FunctionSet.Where(a => a.AppSystem.Id == appSystemID.Value).Select(a => FunctionTr.Create(a)).Where(a => a.IsManaged && a.ResourceTypeID == resource.Id);
            }
            else
            {
                data = new List<FunctionTr>();
            }

            return this.JsonResult(data);
        }

        [By("xuexs")]
        [Description("根据角色ID分页获取权限")]
        public ActionResult GetPlistPrivilegeByRoleID(GetPlistFunctionByRoleID requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = AppHostInstance.GetPlistPrivilegeByRoleID(requestData);

            return this.JsonResult(new MiniGrid<RoleAssignFunctionTr> { total = requestData.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("托管或取消托管功能")]
        [HttpPost]
        public ActionResult ManageOrUnManageFunction()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                var id = new Guid(row["Id"].ToString());
                //根据记录状态，进行不同的增加、删除、修改功能
                String state = row["_state"] != null ? row["_state"].ToString() : "";

                //更新：_state为空或modified
                if (state == "modified" || state == "")
                {
                    bool isManaged = bool.Parse(row["IsManaged"].ToString());
                    var entity = GetRequiredService<IRepository<Function>>().GetByKey(id);
                    if (entity != null)
                    {
                        var input = new FunctionUpdateInput
                        {
                            Description = entity.Description,
                            DeveloperID = entity.DeveloperID,
                            IsEnabled = entity.IsEnabled,
                            IsManaged = entity.IsManaged,
                            SortCode = entity.SortCode,
                            Code = entity.Code,
                            Id = entity.Id
                        };
                        input.IsManaged = isManaged;
                        AppHostInstance.Handle(new UpdateFunctionCommand(input));
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }
    }
}

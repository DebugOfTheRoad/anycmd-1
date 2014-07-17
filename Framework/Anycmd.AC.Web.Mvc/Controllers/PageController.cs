
namespace Anycmd.AC.Web.Mvc.Controllers
{
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC.Infra;
    using Host.AC.Infra.Messages;
    using Infra;
    using Infra.ViewModels.AppSystemViewModels;
    using Infra.ViewModels.FunctionViewModels;
    using Infra.ViewModels.PageViewModels;
    using MiniUI;
    using Repositories;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Linq;
    using System.Web.Mvc;
    using Util;
    using ViewModel;

    /// <summary>
    /// 系统页面模型视图控制器<see cref="Page"/>
    /// </summary>
    public class PageController : AnycmdController
    {
        private readonly EntityTypeState pageEntityType;
        private readonly EntityTypeState functionEntityType;

        public PageController()
        {
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Page", out pageEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
            if (!Host.EntityTypeSet.TryGetEntityType("AC", "Function", out functionEntityType))
            {
                throw new CoreException("意外的实体类型");
            }
        }

        #region ViewResults
        [By("xuexs")]
        [Description("页面集")]
        public ViewResultBase Index()
        {
            return ViewResult();
        }

        [By("xuexs")]
        [Description("页面详细信息")]
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

        [By("xuexs")]
        [Description("页面按钮列表")]
        public ViewResultBase PageButtons()
        {
            return ViewResult();
        }

        #endregion

        [By("xuexs")]
        [Description("根据ID获取页面")]
        public ActionResult Get(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(pageEntityType.GetData(id.Value));
        }

        [By("xuexs")]
        [Description("根据ID获取页面详细信息")]
        public ActionResult GetInfo(Guid? id)
        {
            if (!id.HasValue)
            {
                throw new ValidationException("未传入标识");
            }
            return this.JsonResult(FunctionInfo.Create(functionEntityType.GetData(id.Value)));
        }

        [By("xuexs")]
        [Description("获取页面提示信息")]
        public ActionResult Tooltip(Guid? pageID)
        {
            if (pageID.HasValue)
            {
                PageState page;
                IFunction function;
                if (!Host.PageSet.TryGetPage(pageID.Value, out page))
                {
                    page = PageState.Empty;
                    function = new Function();
                }
                else
                {
                    FunctionState FunctionState;
                    if (!Host.FunctionSet.TryGetFunction(page.Id, out FunctionState))
                    {
                        throw new ValidationException("意外的功能标识" + page.Id);
                    }
                    function = FunctionState;
                }
                return this.PartialView("Partials/Tooltip", new PageViewModel(page, function.Description));
            }
            else
            {
                return this.Content("无效的pageID");
            }
        }

        [By("xuexs")]
        [Description("编辑页面帮助")]
        public ActionResult TooltipEdit(string isInner, Guid? pageID)
        {
            if (pageID.HasValue)
            {
                PageState page;
                IFunction function;
                if (!Host.PageSet.TryGetPage(pageID.Value, out page))
                {
                    page = PageState.Empty;
                    function = new Function();
                }
                else
                {
                    FunctionState FunctionState;
                    if (!Host.FunctionSet.TryGetFunction(page.Id, out FunctionState))
                    {
                        throw new ValidationException("意外的功能标识" + page.Id);
                    }
                    function = FunctionState;
                }
                return this.PartialView(new PageViewModel(page, function.Description));
            }
            else
            {
                return this.Content("无效的pageID");
            }
        }

        [By("xuexs")]
        [Description("编辑页面帮助")]
        [ValidateInput(enableValidation: false)]
        public ActionResult SaveTooltip(string tooltip, Guid? pageID)
        {
            if (!pageID.HasValue)
            {
                throw new ValidationException("非法的页面标识" + pageID);
            }
            var entity = GetRequiredService<IRepository<Page>>().GetByKey(pageID.Value);
            if (entity == null)
            {
                throw new ValidationException("标识为" + pageID + "的页面不存在");
            }
            Host.Handle(new UpdatePageCommand(new PageUpdateInput
            {
                Icon = entity.Icon,
                Id = entity.Id,
                Tooltip = tooltip
            }));
            return this.JsonResult(new ResponseData { success = true });
        }

        [By("xuexs")]
        [Description("分页获取页面")]
        public ActionResult GetPlistPages(GetPlistResult requestData)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            var data = Host.GetPlistPages(requestData);

            return this.JsonResult(new MiniGrid<PageTr> { total = requestData.total.Value, data = data });
        }

        [By("xuexs")]
        [Description("添加页面")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Create(PageCreateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.Handle(new AddPageCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("更新页面")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult Update(PageUpdateInput input)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToJsonResult();
            }
            Host.Handle(new UpdatePageCommand(input));

            return this.JsonResult(new ResponseData { id = input.Id, success = true });
        }

        [By("xuexs")]
        [Description("添加或移除页面按钮")]
        [HttpPost]
        [DeveloperFilter(Order = 21)]
        public ActionResult AddOrRemoveButtons()
        {
            String json = Request["data"];
            var rows = (ArrayList)MiniJSON.Decode(json);
            foreach (Hashtable row in rows)
            {
                //根据记录状态，进行不同的增加、删除、修改操作
                String state = row["_state"] != null ? row["_state"].ToString() : "";

                //更新：_state为空或modified
                if (state == "modified" || state == "")
                {
                    string functionIDStr = row["FunctionID"] == null ? null : row["FunctionID"].ToString();
                    Guid? functionID = string.IsNullOrEmpty(functionIDStr) ? null : new Nullable<Guid>(new Guid(functionIDStr));
                    if (functionID.HasValue)
                    {
                        FunctionState function;
                        if (!Host.FunctionSet.TryGetFunction(functionID.Value, out function))
                        {
                            throw new ValidationException("意外的托管功能标识" + functionID.Value);
                        }
                    }
                    var inputModel = new PageButton
                    {
                        Id = new Guid(row["Id"].ToString()),
                        IsEnabled = int.Parse(row["IsEnabled"].ToString()),
                        ButtonID = new Guid(row["ButtonID"].ToString()),
                        PageID = new Guid(row["PageID"].ToString()),
                        FunctionID = functionID
                    };

                    if (bool.Parse(row["IsAssigned"].ToString()))
                    {
                        if (Host.GetRequiredService<IRepository<PageButton>>().FindAll().Any(a => a.Id == inputModel.Id))
                        {
                            var updateModel = new PageButtonUpdateInput()
                            {
                                Id = inputModel.Id,
                                IsEnabled = inputModel.IsEnabled,
                                FunctionID = inputModel.FunctionID
                            };
                            Host.Handle(new UpdatePageButtonCommand(updateModel));
                        }
                        else
                        {
                            var input = new PageButtonCreateInput()
                            {
                                Id = inputModel.Id,
                                ButtonID = inputModel.ButtonID,
                                IsEnabled = inputModel.IsEnabled,
                                FunctionID = inputModel.FunctionID,
                                PageID = inputModel.PageID
                            };
                            Host.Handle(new AddPageButtonCommand(input));
                        }
                    }
                    else
                    {
                        Host.Handle(new RemovePageButtonCommand(inputModel.Id));
                    }
                    if (functionID.HasValue)
                    {
                        int functionIsEnabled = int.Parse(row["FunctionIsEnabled"].ToString());
                        FunctionState function;
                        if (!Host.FunctionSet.TryGetFunction(functionID.Value, out function))
                        {
                            throw new CoreException("意外的功能标识" + functionID.Value);
                        }
                        var input = new FunctionUpdateInput
                        {
                            Id = function.Id,
                            Code = function.Code,
                            SortCode = function.SortCode,
                            IsManaged = function.IsManaged,
                            IsEnabled = function.IsEnabled,
                            DeveloperID = function.DeveloperID,
                            Description = function.Description
                        };
                        input.IsEnabled = functionIsEnabled;
                        Host.Handle(new UpdateFunctionCommand(input));
                    }
                }
            }

            return this.JsonResult(new ResponseData { success = true });
        }

        [By("xuexs")]
        [Description("删除页面")]
        [HttpPost]
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
                    throw new ValidationException("意外的应用系统标识" + ids[i]);
                }
            }
            foreach (var item in idArray)
            {
                Host.Handle(new RemovePageCommand(item));
            }

            return this.JsonResult(new ResponseData { id = id, success = true });
        }
    }
}

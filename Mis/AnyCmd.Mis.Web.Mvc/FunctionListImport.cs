
namespace Anycmd.Mis.Web.Mvc
{
    using AC;
    using AC.Infra;
    using AC.Infra.ViewModels.AppSystemViewModels;
    using AC.Infra.ViewModels.PageViewModels;
    using Anycmd.Web.Mvc;
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Infra.Messages;
    using Host.AC.Messages;
    using Repositories;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using Util;

    /// <summary>
    /// 暂时支持的是导入本系统的功能列表，数据源是通过反射控制器层得到的，后续支持以xml文件的方式导入其它系统的功能列表
    /// </summary>
    public class FunctionListImport : IFunctionListImport
    {
        private static readonly object locker = new object();
        private static bool _isChanged = true;

        public void Import(AppHost host, string appSystemCode)
        {
            if (_isChanged)
            {
                lock (locker)
                {
                    if (_isChanged)
                    {
                        var privilegeBigramRepository = host.GetRequiredService<IRepository<PrivilegeBigram>>();
                        if (string.IsNullOrEmpty(appSystemCode))
                        {
                            throw new ArgumentNullException("appSystemCode");
                        }
                        AppSystemState appSystem;
                        if (!host.AppSystemSet.TryGetAppSystem(appSystemCode, out appSystem))
                        {
                            throw new ValidationException("意外的应用系统码" + appSystemCode);
                        }
                        // TODO：提取配置
                        var assemblyStrings = new List<string> {
                            "Anycmd.AC.Web.Mvc",
                            "Anycmd.EDI.Web.Mvc",
                            "Anycmd.Mis.Web.Mvc"
                        };
                        var dlls = new List<Assembly>();
                        foreach (var assemblyString in assemblyStrings)
                        {
                            dlls.Add(Assembly.Load(assemblyString));
                        }
                        var oldPages = host.PageSet;
                        var oldFunctions = new List<IFunction>();
                        foreach (var function in host.FunctionSet)
                        {
                            oldFunctions.Add(function);
                        }
                        var reflectionFunctions = new List<FunctionID>();
                        #region 通过反射程序集初始化功能和页面列表
                        foreach (var dll in dlls)
                        {
                            // 注意这里约定区域名为二级命名空间的名字
                            var areaCode = dll.GetName().Name.Split('.')[1];

                            var types = dll.GetTypes();
                            var actionResultType = typeof(ActionResult);
                            var controllerType = typeof(AnycmdController);
                            var viewResultType = typeof(ViewResultBase);
                            foreach (var type in types)
                            {
                                bool isController = controllerType.IsAssignableFrom(type);
                                // 跳过不是Controller的类型
                                if (!isController)
                                {
                                    continue;
                                }
                                var controller = type.Name.Substring(0, type.Name.Length - "Controller".Length);
                                var resourceCode = controller;// 注意这里约定资源码等于控制器名
                                var methodInfos = type.GetMethods();
                                int sortCode = 10;
                                foreach (var method in methodInfos)
                                {
                                    bool isPage = viewResultType.IsAssignableFrom(method.ReturnType);
                                    bool isAction = isPage || actionResultType.IsAssignableFrom(method.ReturnType);
                                    string action = method.Name;

                                    ResourceTypeState resource = ResourceTypeState.Empty;
                                    var description = string.Empty;
                                    Guid developerID = Guid.Empty;
                                    // 跳过不是Action的方法
                                    if (!isAction)
                                    {
                                        continue;
                                    }
                                    object[] descriptionAttrs = method.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), inherit: false);
                                    // 如果打有描述文本标记则使用描述文本作为操作说明，否则试用Action方法名
                                    if (descriptionAttrs.Length > 0)
                                    {
                                        description = (descriptionAttrs[0] as System.ComponentModel.DescriptionAttribute).Description;
                                        if (string.IsNullOrEmpty(description))
                                        {
                                            description = action;
                                        }
                                    }
                                    else
                                    {
                                        description = action;
                                    }
                                    object[] byAttrs = method.GetCustomAttributes(typeof(ByAttribute), inherit: false);
                                    if (byAttrs.Length > 0)
                                    {
                                        string loginName = (byAttrs[0] as ByAttribute).DeveloperCode;
                                        AccountState developer;
                                        if (!host.SysUsers.TryGetDevAccount(loginName, out developer))
                                        {
                                            throw new ValidationException("意外的开发人员" + loginName + "在" + controllerType.FullName + "在" + method.Name);
                                        }
                                        developerID = developer.Id;
                                    }
                                    else
                                    {
                                        throw new ValidationException(type.FullName + method.Name);
                                    }

                                    if (!host.ResourceSet.TryGetResource(appSystem, resourceCode, out resource))
                                    {
                                        throw new ValidationException("意外的资源码" + resourceCode);
                                    }
                                    var oldFunction = oldFunctions.FirstOrDefault(
                                            o => o.ResourceTypeID == resource.Id
                                            && string.Equals(o.Code, action, StringComparison.OrdinalIgnoreCase));
                                    if (oldFunction == null)
                                    {
                                        var function = FunctionID.Create(Guid.NewGuid(), appSystemCode, areaCode, resourceCode, action);
                                        if (reflectionFunctions.Any(a => a.AreaCode == areaCode && a.ResourceCode == resourceCode && string.Equals(a.FunctionCode, action, StringComparison.OrdinalIgnoreCase)))
                                        {
                                            throw new ValidationException("同一Controller下不能有命名相同的Action。" + method.DeclaringType.FullName + "." + method.Name);
                                        }
                                        reflectionFunctions.Add(function);
                                        host.Handle(new AddFunctionCommand(new FunctionCreateInput()
                                        {
                                            Description = description,
                                            DeveloperID = developerID,
                                            Id = function.Id,
                                            IsEnabled = 1,
                                            IsManaged = false,
                                            ResourceTypeID = resource.Id,
                                            SortCode = sortCode,
                                            Code = function.FunctionCode
                                        }));
                                        if (isPage)
                                        {
                                            host.Handle(new AddPageCommand(new PageCreateInput
                                            {
                                                Id = function.Id
                                            }));
                                        }
                                    }
                                    else
                                    {
                                        // 更新作者
                                        if (oldFunction.DeveloperID != developerID)
                                        {
                                            host.Handle(new UpdateFunctionCommand(new FunctionUpdateInput
                                            {
                                                Code = oldFunction.Code,
                                                Description = oldFunction.Description,
                                                DeveloperID = developerID,
                                                Id = oldFunction.Id,
                                                SortCode = oldFunction.SortCode
                                            }));
                                        }
                                        reflectionFunctions.Add(FunctionID.Create(oldFunction.Id, appSystemCode, areaCode, resourceCode, action));
                                        if (isPage)
                                        {
                                            if (!oldPages.Any(a => a.Id == oldFunction.Id))
                                            {
                                                host.Handle(new AddPageCommand(new PageCreateInput
                                                {
                                                    Id = oldFunction.Id
                                                }));
                                            }
                                        }
                                        else
                                        {
                                            // 删除废弃的页面
                                            if (!oldPages.Any(a => a.Id == oldFunction.Id))
                                            {
                                                host.Handle(new RemovePageCommand(oldFunction.Id));
                                            }
                                        }
                                    }
                                    sortCode += 10;
                                }
                            }
                        }
                        #endregion
                        #region 删除废弃的功能
                        foreach (var oldFunction in oldFunctions)
                        {
                            if (!reflectionFunctions.Any(o => o.Id == oldFunction.Id))
                            {
                                // 删除角色功能
                                var privilegeType = ACObjectType.Function.ToName();
                                foreach (var rolePrivilege in privilegeBigramRepository.FindAll().Where(a => privilegeType == a.ObjectType && a.ObjectInstanceID == oldFunction.Id).ToList())
                                {
                                    privilegeBigramRepository.Remove(rolePrivilege);
                                    host.EventBus.Publish(new PrivilegeBigramRemovedEvent(rolePrivilege));
                                }
                                host.EventBus.Commit();
                                privilegeBigramRepository.Context.Commit();
                                host.Handle(new RemoveFunctionCommand(oldFunction.Id));
                            }
                        }
                        #endregion
                        _isChanged = false;
                    }
                }
            }
        }

        private struct FunctionID
        {
            public static FunctionID Create(Guid id, string appSystemCode, string areaCode, string resourceCode, string functionCode)
            {
                return new FunctionID
                {
                    Id = id,
                    AppSystemCode = appSystemCode,
                    AreaCode = areaCode,
                    ResourceCode = resourceCode,
                    FunctionCode = functionCode
                };
            }

            public Guid Id { get; private set; }
            public string AppSystemCode { get; private set; }
            public string AreaCode { get; private set; }
            public string ResourceCode { get; private set; }
            public string FunctionCode { get; private set; }
        }
    }
}

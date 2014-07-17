﻿
namespace Anycmd.Web.Mvc
{
    using EDI;
    using Exceptions;
    using Host;
    using Host.EDI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using ViewModel;

    /// <summary>
    /// 只提供ViewModel，不提供视图。即这里的扩展方法应只返回数据，不返回Html。
    /// </summary>
    public static class ViewModelExtensions
    {
        #region AC
        private const string FIELD_DIC = "PropertyDic_{0}_{1}";

        #region DatabaseJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="categoryCode"></param>
        /// <returns></returns>
        public static IHtmlString DatabaseJsonArray(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            foreach (var item in html.GetHost().Rdbs)
            {
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{")
                    .Append("'id':").Append("'").Append(item.Database.Id).Append("'")
                    .Append(",'catalogName':").Append("'").Append(item.Database.CatalogName).Append("'");
                sb.Append("}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region DeveloperJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString DeveloperJsonArray(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            foreach (var item in html.GetHost().SysUsers.GetDevAccounts())
            {
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{")
                    .Append("'id':").Append("'").Append(item.Id).Append("'")
                    .Append(",'code':").Append("'").Append(item.LoginName).Append("'")
                    .Append(",'name':").Append("'").Append(item.LoginName).Append("'");
                sb.Append("}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region DicJsonArray
        /// <summary>
        /// <code>
        /// {
        ///     'id':'DCA0E037-D4AE-4A4E-B987-06240353574B',
        ///     'code':'rdbmsType',
        ///     'name':'rdbmsType'
        /// }
        /// </code>
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString DicJsonArray(this HtmlHelper html)
        {
            var value = "[]";
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            foreach (var item in html.GetHost().DicSet.OrderBy(a => a.SortCode))
            {
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{'id':").Append("'").Append(item.Id).Append("'")
                    .Append(",'code':").Append("'").Append(item.Code).Append("'")
                    .Append(",'name':'").Append(item.Code).Append(" | ").Append(item.Name).Append("'}");
            }
            sb.Append("]");
            value = sb.ToString();

            return html.Raw(value);
        }
        #endregion

        #region DicItemJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="dicCode"></param>
        /// <returns></returns>
        public static IHtmlString DicItemJsonArray(this HtmlHelper html, string dicCode)
        {
            DicState dic;
            if (!html.GetHost().DicSet.TryGetDic(dicCode, out dic))
            {
                throw new CoreException("意外的字典编码" + dicCode);
            }
            var value = "[]";
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            var dicItems = html.GetHost().DicSet.GetDicItems(dic);
            foreach (var item in dicItems)
            {
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{'code':").Append("'").Append(item.Value.Code).Append("'")
                    .Append(",'name':'").Append(item.Value.Name).Append(" | ").Append(item.Value.Code).Append("'}");
            }
            sb.Append("]");
            value = sb.ToString();

            return html.Raw(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IHtmlString DicItemJsonArray(this HtmlHelper html, PropertyState property)
        {
            var host = html.ViewContext.HttpContext.Application["AppHostInstance"] as AppHost;
            if (property.DicID.HasValue)
            {
                DicState dic;
                if (host.DicSet.TryGetDic(property.DicID.Value, out dic))
                {
                    return DicItemJsonArray(html, dic.Code);
                }
            }

            return html.Raw("[]");
        }
        #endregion

        #region iconLabel

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="propertyCode"></param>
        /// <returns></returns>
        public static IHtmlString IconLabel(this HtmlHelper html, string propertyCode)
        {
            var entityTypeCode = html.ViewContext.RouteData.Values["Controller"].ToString();

            return IconLabel(html, propertyCode, entityTypeCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="propertyCode"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        public static IHtmlString IconLabel(this HtmlHelper html, string propertyCode, string entityTypeCode, bool joint = false)
        {
            var areaCode = html.ViewContext.RouteData.DataTokens["area"].ToString();

            return IconLabel(html, propertyCode, entityTypeCode, areaCode, joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="propertyCode"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="codespace"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        public static IHtmlString IconLabel(this HtmlHelper html, string propertyCode, string entityTypeCode, string codespace, bool joint = false)
        {
            var field = GetProperty(html, propertyCode, entityTypeCode, codespace);
            return IconLabel(html, field, joint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="property"></param>
        /// <param name="joint"></param>
        /// <returns></returns>
        public static IHtmlString IconLabel(this HtmlHelper html, PropertyState property, bool joint = false)
        {
            IHtmlString result = null;
            var propertyName = property.Name;
            if (joint)
            {
                EntityTypeState entityType;
                html.GetHost().EntityTypeSet.TryGetEntityType(property.EntityTypeID, out entityType);
                propertyName = entityType.Name + propertyName;
            }
            result = string.IsNullOrEmpty(property.Icon) ? html.Raw(string.Format("<label>{0}</label>", property.Name)) : html.Raw(string.Format("<img src='/content/icons/16x16/{0}' /><label>{1}</label>", property.Icon, propertyName));

            return result;
        }

        #endregion

        #region qtip

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="propertyCode"></param>
        /// <returns></returns>
        public static IHtmlString Qtip(this HtmlHelper html, string propertyCode)
        {
            var entityTypeCode = html.ViewContext.RouteData.Values["Controller"].ToString();

            return Qtip(html, propertyCode, entityTypeCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="propertyCode"></param>
        /// <param name="entityTypeCode"></param>
        /// <returns></returns>
        public static IHtmlString Qtip(this HtmlHelper html, string propertyCode, string entityTypeCode)
        {
            string areaCode = html.ViewContext.RouteData.DataTokens["area"].ToString();

            return Qtip(html, propertyCode, entityTypeCode, areaCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="propertyCode"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="codespace"></param>
        /// <returns></returns>
        public static IHtmlString Qtip(this HtmlHelper html, string propertyCode, string entityTypeCode, string codespace)
        {
            IHtmlString result = MvcHtmlString.Empty;
            var property = GetProperty(html, propertyCode, entityTypeCode, codespace);
            if (property.EntityTypeID == EntityTypeState.Empty.Id)
            {
                return result;
            }
            if (property != null && (!string.IsNullOrEmpty(property.Tooltip) || property.AppHost.User.IsDeveloper()))
            {
                var urlHelper = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);
                var href = urlHelper.Action("Tooltip", "Property", new { area = "AC", propertyID = property.Id });
                var aTip = @"<a class='tooltip fieldTooltip' tabIndex='-1' href='{0}' rel='{0}' title='{1}'><b>?</b></a>";

                result = html.Raw(string.Format(aTip, href, property.Name));
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static IHtmlString Qtip(this HtmlHelper html, PageState page)
        {
            IHtmlString result = MvcHtmlString.Empty;
            if (page == PageState.Empty)
            {
                return result;
            }
            if (page != null && (!string.IsNullOrEmpty(page.Tooltip) || page.AppHost.User.IsDeveloper()))
            {
                var urlHelper = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);
                var href = urlHelper.Action("Tooltip", "Page", new { area = "AC", pageID = page.Id });
                var s = "<a class='tooltip pageTooltip' tabIndex='-1' href='{0}' rel='{0}' title='{1}'><b>?</b></a>";
                FunctionState function;
                html.GetHost().FunctionSet.TryGetFunction(page.Id, out function);
                string title = "未知页面";
                if (!function.Equals(FunctionState.Empty))
                {
                    title = function.Description;
                }
                result = html.Raw(string.Format(s, href, title));
            }

            return result;
        }

        #endregion

        #region runtimePage
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static PageViewModel GetRuntimePage(this HtmlHelper html, string action)
        {
            var controller = html.ViewContext.RouteData.Values["Controller"].ToString();

            return GetRuntimePage(html, action, controller);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewPage"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static PageViewModel GetRuntimePage(this WebViewPage viewPage, string action)
        {
            return GetRuntimePage(viewPage.Html, action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewPage"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static PageViewModel GetRuntimePage(this WebViewPage viewPage, string action, string controller)
        {
            return GetRuntimePage(viewPage.Html, action, controller);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static PageViewModel GetRuntimePage(this HtmlHelper html, string action, string controller)
        {
            var host = html.GetHost();
            ResourceTypeState resource;
            if (!host.ResourceSet.TryGetResource(host.AppSystemSet.SelfAppSystem, controller, out resource))
            {
                return new PageViewModel(PageState.Empty, "未知页面");
            }
            FunctionState function;
            if (!host.FunctionSet.TryGetFunction(resource, action, out function))
            {
                return new PageViewModel(PageState.Empty, "未知页面");
            }
            PageState page;
            if (!host.PageSet.TryGetPage(function, out page))
            {
                return new PageViewModel(PageState.Empty, "未知页面");
            }
            string title = function.Description;

            return new PageViewModel(page, title);
        }
        #endregion

        #region GetFunction
        public static FunctionState GetFunction(this HtmlHelper html, Guid functionID)
        {
            FunctionState function;
            if (!html.GetHost().FunctionSet.TryGetFunction(functionID, out function))
            {
                throw new CoreException("意外的按钮功能标识" + functionID);
            }
            return function;
        }
        #endregion

        #region IsEnabled
        public static IHtmlString IsEnabled(this HtmlHelper html, string resourceCode, string functionCode)
        {
            var htmlEnabled = string.Empty;
            if (!html.GetHost().User.Permit(resourceCode, functionCode))
            {
                htmlEnabled = "enabled='false'";
            }
            return html.Raw(htmlEnabled);
        }
        #endregion

        #region GetEntityType
        public static EntityTypeState GetEntityType(this HtmlHelper html, string codespace, string entityTypeCode)
        {
            EntityTypeState entityType;
            if (!html.GetHost().EntityTypeSet.TryGetEntityType(codespace, entityTypeCode, out entityType))
            {
                throw new CoreException(string.Format("意外的实体类型码{0}.{1}", codespace, entityTypeCode));
            }
            return entityType;
        }
        #endregion

        #region GetProperty
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="propertyCode"></param>
        /// <returns>返回NoneProperty，不会返回null</returns>
        public static PropertyState GetProperty(this HtmlHelper html, string propertyCode)
        {
            var fieldDic = GetPropertyDic(html);
            if (fieldDic != null && fieldDic.ContainsKey(propertyCode))
            {
                return fieldDic[propertyCode];
            }
            else
            {
                return PropertyState.CreateNoneProperty(propertyCode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="propertyCode"></param>
        /// <returns>返回NoneProperty，不会返回null</returns>
        public static PropertyState GetProperty(this HtmlHelper html, string propertyCode, string entityTypeCode)
        {
            var fieldDic = GetPropertyDic(html, entityTypeCode);
            if (fieldDic != null && fieldDic.ContainsKey(propertyCode))
            {
                return fieldDic[propertyCode];
            }
            else
            {
                return PropertyState.CreateNoneProperty(propertyCode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="codespace"></param>
        /// <param name="entityTypeCode"></param>
        /// <param name="propertyCode"></param>
        /// <returns>返回NoneProperty，不会返回null</returns>
        public static PropertyState GetProperty(this HtmlHelper html, string propertyCode, string entityTypeCode, string codespace)
        {
            var fieldDic = GetPropertyDic(html, entityTypeCode, codespace);
            if (fieldDic != null && fieldDic.ContainsKey(propertyCode))
            {
                return fieldDic[propertyCode];
            }
            else
            {
                return PropertyState.CreateNoneProperty(propertyCode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, PropertyState> GetPropertyDic(this HtmlHelper html)
        {
            string areaCode = html.ViewContext.RouteData.DataTokens["area"].ToString();
            var entityTypeCode = html.ViewContext.RouteData.Values["Controller"].ToString();
            return GetPropertyDic(html, entityTypeCode, areaCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="entityTypeCode"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, PropertyState> GetPropertyDic(this HtmlHelper html, string entityTypeCode)
        {
            string areaCode = html.ViewContext.RouteData.DataTokens["area"].ToString();
            return GetPropertyDic(html, entityTypeCode, areaCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="codespace"></param>
        /// <param name="entityTypeCode"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, PropertyState> GetPropertyDic(this HtmlHelper html, string entityTypeCode, string codespace)
        {
            var key = string.Format(FIELD_DIC, codespace, entityTypeCode);
            var obj = html.ViewData[key];
            if (obj == null)
            {
                var host = html.GetHost();
                EntityTypeState entityType;
                if (!host.EntityTypeSet.TryGetEntityType(codespace, entityTypeCode, out entityType))
                {
                    throw new CoreException("意外的实体类型" + codespace + entityTypeCode);
                }
                var propertyDic = host.EntityTypeSet.GetProperties(entityType);
                html.ViewData.Add(key, propertyDic);

                return propertyDic;
            }

            return obj as IReadOnlyDictionary<string, PropertyState>;
        }

        #endregion

        #region Permit
        public static bool Permit(this PageState page)
        {
            return page.AppHost.User.Permit(page);
        }

        public static bool Permit(this PageViewModel page)
        {
            return page.Page.AppHost.User.Permit(page.Page);
        }
        #endregion

        public static AppHost GetHost(this HtmlHelper html)
        {
            return html.ViewContext.HttpContext.Application["AppHostInstance"] as AppHost;
        }

        public static IUserSession GetUser(this WebViewPage page)
        {
            return page.Html.GetHost().User;
        }

        #region GetOperationLogEntityType
        public static PageViewModel GetOperationLogEntityType(this WebViewPage webPage)
        {
            var host = GetHost(webPage.Html);
            ResourceTypeState resource;
            if (!host.ResourceSet.TryGetResource(host.AppSystemSet.SelfAppSystem, "OperationLog", out resource))
            {
                return PageViewModel.Empty;
            }
            FunctionState function;
            if (!host.FunctionSet.TryGetFunction(resource, "OperationLogs", out function))
            {
                return PageViewModel.Empty;
            }
            PageState page;
            if (!host.PageSet.TryGetPage(function, out page))
            {
                return PageViewModel.Empty;
            }
            return new PageViewModel(page, function.Description);
        }
        #endregion
        #endregion

        #region EDI
        #region EntityProvidersJsonArray
        /// <summary>
        /// 返回实体提供程序集的视图模型，以键json数组数组表现：
        /// [{'id':'00690A63-CB26-44D2-B237-8C30D88D30CB', 'title':'SqlServer2008R2'}]
        /// <remarks>
        /// 注意：实体提供程序的标识是Guid类型的，大小写保持原样。
        /// </remarks>
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString EntityProvidersJsonArray(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            foreach (var item in NodeHost.Instance.EntityProviders)
            {
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{")
                    .Append("'id':").Append("'").Append(item.Id).Append("'")
                    .Append(",'title':").Append("'").Append(item.Title).Append("'");
                sb.Append("}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region MessageProvidersJsonArray
        /// <summary>
        /// 返回命令提供程序集的视图模型，以键json数组数组表现：
        /// [{'id':'00690A63-CB26-44D2-B237-8C30D88D30CB', 'title':'SqlServer2008R2'}]
        /// <remarks>
        /// 注意：命令提供程序的标识是Guid类型的，大小写保持原样。
        /// </remarks>
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString MessageProvidersJsonArray(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            foreach (var item in NodeHost.Instance.MessageProviders)
            {
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{")
                    .Append("'id':").Append("'").Append(item.Id).Append("'")
                    .Append(",'title':").Append("'").Append(item.Title).Append("'");
                sb.Append("}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region InfoDicItemsJsonArray
        /// <summary>
        /// 返回根据当前IElement类型的ViewModel拼接的json字符串。格式为[{'code':'01','name':'汉'},{'code':'02','name':'蒙古族'}]
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString InfoDicItemsJsonArray(this HtmlHelper<IElement> html)
        {
            var element = html.ViewData.Model;
            return InfoDicItemsJsonArray(html, element);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="infoDicCode"></param>
        /// <returns></returns>
        public static IHtmlString InfoDicItemsJsonArray(this HtmlHelper html, string infoDicCode)
        {
            InfoDicState infoDic;
            if (!NodeHost.Instance.InfoDics.TryGetInfoDic(infoDicCode, out infoDic))
            {
                return html.Raw("[]");
            }
            var dicItems = NodeHost.Instance.InfoDics.GetInfoDicItems(infoDic);
            return InfoDicItemsJsonArray(html, dicItems);
        }

        /// <summary>
        /// 返回根据给定的IElement拼接的json字符串。格式为[{'code':'01','name':'汉'},{'code':'02','name':'蒙古族'}]
        /// </summary>
        /// <param name="html"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IHtmlString InfoDicItemsJsonArray(this HtmlHelper html, IElement element)
        {
            if (element.InfoDicID.HasValue)
            {
                InfoDicState infoDic;
                if (!NodeHost.Instance.InfoDics.TryGetInfoDic(element.InfoDicID.Value, out infoDic))
                {
                    return html.Raw("[]");
                }
                var dicItems = NodeHost.Instance.InfoDics.GetInfoDicItems(infoDic);
                return InfoDicItemsJsonArray(html, dicItems);
            }

            return html.Raw("[]");
        }

        public static IHtmlString InfoDIcJsonArray(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            foreach (var item in NodeHost.Instance.InfoDics)
            {
                if (item.IsEnabled != 1)
                {
                    continue;
                }
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{")
                    .Append("'id':").Append("'").Append(item.Id).Append("'")
                    .Append(",'code':").Append("'").Append(item.Code).Append("'")
                    .Append(",'name':").Append("'").Append(item.Code).Append(" | ").Append(item.Name).Append("'");
                sb.Append("}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }

        private static IHtmlString InfoDicItemsJsonArray(HtmlHelper html, IEnumerable<IInfoDicItem> dicItems)
        {
            if (dicItems == null)
            {
                return html.Raw("[]");
            }
            var sb = new StringBuilder();
            sb.Append("[");
            int l = sb.Length;
            foreach (var item in dicItems)
            {
                if (item.IsEnabled != 1)
                {
                    continue;
                }
                if (sb.Length > l)
                {
                    sb.Append(",");
                }
                sb.Append("{")
                    .Append("'code':").Append("'").Append(item.Code).Append("'")
                    .Append(",'name':").Append("'").Append(item.Code).Append(" | ").Append(item.Name).Append("'");
                sb.Append("}");
            }
            sb.Append("]");
            return html.Raw(sb.ToString());
        }
        #endregion

        #region NodesJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString NodesJsonArray(this HtmlHelper html)
        {
            StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("[");
            var l = sb.Length;
            foreach (var node in NodeHost.Instance.Nodes)
            {
                if (sb.Length != l)
                {
                    sb.Append(",");
                }
                sb.Append("{id:'").Append(node.Node.Id.ToString())
                    .Append("',name:'").Append(node.Node.Name).Append("'}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region ClientNodesJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString ClientNodesJsonArray(this HtmlHelper html)
        {
            StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("[");
            var l = sb.Length;
            foreach (var node in NodeHost.Instance.Nodes)
            {
                if (node.Node.Id != NodeHost.Instance.Nodes.ThisNode.Node.Id)
                {
                    if (sb.Length != l)
                    {
                        sb.Append(",");
                    }
                    sb.Append("{id:'").Append(node.Node.Id.ToString())
                        .Append("',name:'").Append(node.Node.Name).Append("'}");
                }
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region ActionsJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="ontology"></param>
        /// <returns></returns>
        public static IHtmlString ActionsJsonArray(this HtmlHelper html, OntologyDescriptor ontology)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var l = sb.Length;
            foreach (var action in ontology.Actions)
            {
                if (sb.Length != l)
                {
                    sb.Append(",");
                }
                sb.Append("{code:'").Append(action.Key)
                    .Append("',name:'").Append(action.Key).Append(" | ").Append(action.Value.Name).Append("'}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region InfoFormatJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString InfoFormatJsonArray(this HtmlHelper html)
        {
            var infoValuesConverters = NodeHost.Instance.InfoStringConverters;
            var sb = new StringBuilder();
            sb.Append("[");
            var l = sb.Length;
            foreach (var converter in infoValuesConverters)
            {
                if (sb.Length != l)
                {
                    sb.Append(",");
                }
                sb.Append("{code:'").Append(converter.InfoFormat)
                    .Append("',name:'").Append(converter.InfoFormat).Append("'}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region TransfersJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString TransfersJsonArray(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var l = sb.Length;
            foreach (var strategy in NodeHost.Instance.Transfers)
            {
                if (sb.Length != l)
                {
                    sb.Append(",");
                }
                sb.Append("{Id:'").Append(strategy.Id)
                    .Append("',Title:'").Append(strategy.Title).Append("'}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region OntologiesJsonArray
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static IHtmlString OntologiesJsonArray(this HtmlHelper html)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var l = sb.Length;
            foreach (var ontology in NodeHost.Instance.Ontologies)
            {
                if (sb.Length != l)
                {
                    sb.Append(",");
                }
                sb.Append("{code:'").Append(ontology.Ontology.Code)
                    .Append("',id:'").Append(ontology.Ontology.Id)
                    .Append("',name:'").Append(ontology.Ontology.Code).Append(" | ").Append(ontology.Ontology.Name).Append("'}");
            }
            sb.Append("]");

            return html.Raw(sb.ToString());
        }
        #endregion

        #region GetOntology
        public static OntologyDescriptor GetOntology(this HtmlHelper html, string ontologyCode)
        {
            OntologyDescriptor ontology;
            if (!NodeHost.Instance.Ontologies.TryGetOntology(ontologyCode, out ontology))
            {
                throw new ValidationException("意外的本体码" + ontologyCode);
            }
            return ontology;
        }
        #endregion

        #region iconLabel

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IHtmlString IconLabel(this HtmlHelper html, IElement element)
        {
            IHtmlString result = null;
            result = string.IsNullOrEmpty(element.Icon) ? html.Raw(string.Format("<label>{0}</label>", element.Name)) : html.Raw(string.Format("<img src='/content/icons/16x16/{0}' /><label>{1}</label>", element.Icon, element.Name));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IHtmlString Label(this HtmlHelper html, IElement element)
        {
            IHtmlString result = null;
            result = html.Raw(string.Format("<label>{0}</label>", element.Name));

            return result;
        }

        #endregion

        #region Qtip
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html"></param>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IHtmlString Qtip(this HtmlHelper html, IElement element)
        {
            IHtmlString result = MvcHtmlString.Empty;
            if (element != null && (!string.IsNullOrEmpty(element.Tooltip) || NodeHost.Instance.AppHost.User.IsDeveloper()))
            {
                var urlHelper = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection);
                var href = urlHelper.Action("Tooltip", "Element", new { area = "EDI", elementID = element.Id });
                var aTip = @"<a class='tooltip elementTooltip' tabIndex='-1' href='{0}' rel='{0}' title='{1}'><b>?</b></a>";

                result = html.Raw(string.Format(aTip, href, element.Name));
            }

            return result;
        }

        #endregion
        #endregion
    }
}

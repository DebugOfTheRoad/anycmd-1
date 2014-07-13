
namespace Anycmd.AC.Infra.ViewModels.OrganizationViewModels
{
    using Anycmd.Host;
    using Exceptions;
    using System;
    using System.Collections.Generic;
    using Util;

    public class OrganizationInfo : Dictionary<string, object>
    {
        private OrganizationInfo() { }

        public static OrganizationInfo Create(DicReader dic)
        {
            if (dic == null)
            {
                return null;
            }
            var data = new OrganizationInfo();
            foreach (var item in dic)
            {
                data.Add(item.Key, item.Value);
            }
            if (!data.ContainsKey("CategoryName"))
            {
                data.Add("CategoryName", dic.AppHost.Translate("AC", "Organization", "CategoryName", data["CategoryCode"].ToString()));
            }
            if (data["ParentCode"] != DBNull.Value)
            {
                string parentCode = (string)data["ParentCode"];
                OrganizationState parentOrg;
                if (!dic.AppHost.OrganizationSet.TryGetOrganization(parentCode, out parentOrg))
                {
                    throw new CoreException("意外的父组织结构编码" + parentCode);
                }
                data.Add("ParentName", parentOrg.Name);
            }
            else
            {
                data.Add("ParentName", OrganizationState.VirtualRoot.Name);
            }

            return data;
        }
    }
}

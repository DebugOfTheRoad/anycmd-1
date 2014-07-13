
namespace Anycmd.AC.Infra.ViewModels.DicViewModels
{
    using System.Collections.Generic;
    using Util;

    public class DicItemInfo : Dictionary<string, object>
    {
        private DicItemInfo() { }

        public static DicItemInfo Create(DicReader dic)
        {
            if (dic == null)
            {
                return null;
            }
            var data = new DicItemInfo();
            foreach (var item in dic)
            {
                data.Add(item.Key, item.Value);
            }
            if (!data.ContainsKey("IsEnabledName"))
            {
                data.Add("IsEnabledName", dic.AppHost.Translate("AC", "DicItem", "IsEnabledName", data["IsEnabled"].ToString()));
            }

            return data;
        }
    }
}

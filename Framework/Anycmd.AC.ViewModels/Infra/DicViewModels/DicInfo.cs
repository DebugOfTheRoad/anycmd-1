
namespace Anycmd.AC.Infra.ViewModels.DicViewModels
{
    using System.Collections.Generic;
    using Util;

    public class DicInfo : Dictionary<string, object>
    {
        private DicInfo() { }

        public static DicInfo Create(DicReader dic)
        {
            if (dic == null)
            {
                return null;
            }
            var data = new DicInfo();
            foreach (var item in dic)
            {
                data.Add(item.Key, item.Value);
            }
            if (!data.ContainsKey("IsEnabledName"))
            {
                data.Add("IsEnabledName", dic.AppHost.Translate("AC", "Dic", "IsEnabledName", data["IsEnabled"].ToString()));
            }

            return data;
        }
    }
}

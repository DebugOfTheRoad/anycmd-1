
namespace Anycmd.AC.Infra.ViewModels.ButtonViewModels
{
    using Model;
    using System.Collections.Generic;

    public class ButtonInfo : Dictionary<string, object>
    {
        private ButtonInfo() { }

        public static ButtonInfo Create(DicReader dic)
        {
            if (dic == null)
            {
                return null;
            }
            var data = new ButtonInfo();
            foreach (var item in dic)
            {
                data.Add(item.Key, item.Value);
            }
            if (!data.ContainsKey("IsEnabledName"))
            {
                data.Add("IsEnabledName", dic.Host.Translate("AC", "Button", "IsEnabledName", data["IsEnabled"].ToString()));
            }

            return data;
        }
    }
}

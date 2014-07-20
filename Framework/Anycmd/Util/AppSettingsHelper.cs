
namespace Anycmd.Util
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    /// <summary>
    /// 配置文件访问帮手
    /// </summary>
    public class AppSettingsHelper
    {
        public static readonly AppSettingsHelper Current = new AppSettingsHelper();

        private readonly NameValueCollection appSettings;

        public AppSettingsHelper()
        {
            appSettings = ConfigurationManager.AppSettings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appSettings"></param>
        public AppSettingsHelper(NameValueCollection appSettings)
        {
            this.appSettings = appSettings;
        }

        #region GetString

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name)
        {
            return getValue(name, true, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetString(string name, string defaultValue)
        {
            return getValue(name, false, defaultValue);
        }

        #endregion

        #region GetStringArray

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string[] GetStringArray(string name, string separator)
        {
            return getStringArray(name, separator, true, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="separator"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string[] GetStringArray(string name, string separator, string[] defaultValue)
        {
            return getStringArray(name, separator, false, defaultValue);
        }

        #endregion

        #region GetInt32

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetInt32(string name)
        {
            return getInt32(name, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int GetInt32(string name, int defaultValue)
        {
            return getInt32(name, defaultValue);
        }

        #endregion

        #region GetBoolean

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool GetBoolean(string name)
        {
            return getBoolean(name, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBoolean(string name, bool defaultValue)
        {
            return getBoolean(name, defaultValue);
        }

        #endregion

        #region GetTimeSpan

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public TimeSpan GetTimeSpan(string name)
        {
            return TimeSpan.Parse(getValue(name, true, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public TimeSpan GetTimeSpan(string name, TimeSpan defaultValue)
        {
            string val = getValue(name, false, null);

            if (val == null)
                return defaultValue;

            return TimeSpan.Parse(val);
        }

        #endregion

        #region Private Methods

        private string[] getStringArray(string name, string separator, bool valueRequired, string[] defaultValue)
        {
            string value = getValue(name, valueRequired, null);

            if (!string.IsNullOrEmpty(value))
                return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            else if (!valueRequired)
                return defaultValue;

            throw generateRequiredSettingException(name);
        }

        private int getInt32(string name, int? defaultValue)
        {
            int value;
            Int32.TryParse(appSettings[name], out value);
            if (value == 0 && defaultValue != null)
            {
                return defaultValue.Value;
            }

            return value;
        }

        private bool getBoolean(string name, bool? defaultValue)
        {
            string value = appSettings[name];

            if (value != null)
            {
                bool parsedValue = false;

                if (bool.TryParse(value, out parsedValue))
                    return parsedValue;
                else
                    throw new InvalidOperationException(string.Format("Setting '{0}' was not a valid {1}", name, typeof(bool).FullName));
            }

            if (!defaultValue.HasValue)
                throw generateRequiredSettingException(name);
            else
                return defaultValue.Value;
        }

        private T getValue<T>(string name, Func<string, T, bool> parseValue, T? defaultValue) where T : struct
        {
            string value = appSettings[name];

            if (value != null)
            {
                T parsedValue = default(T);

                if (parseValue(value, parsedValue))
                    return parsedValue;
                else
                    throw new InvalidOperationException(string.Format("Setting '{0}' was not a valid {1}", name, typeof(T).FullName));
            }

            if (!defaultValue.HasValue)
                throw generateRequiredSettingException(name);
            else
                return defaultValue.Value;
        }

        private string getValue(string name, bool valueRequired, string defaultValue)
        {
            string value = appSettings[name];

            if (value != null)
                return value;
            else if (!valueRequired)
                return defaultValue;

            throw generateRequiredSettingException(name);
        }

        private Exception generateRequiredSettingException(string name)
        {
            return new InvalidOperationException(string.Format("Could not find required setting '{0}'", name));
        }

        #endregion
    }
}

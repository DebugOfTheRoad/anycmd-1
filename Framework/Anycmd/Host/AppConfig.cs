
namespace Anycmd.Host
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// 数据交换平台配置信息访问器，该访问器在第一次使用之前就绪。
    /// </summary>
    public sealed class AppConfig : IAppConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public AppConfig(List<IParameter> parms)
        {
            this.Parameters = parms;
            this.Init();
        }

        public IReadOnlyCollection<IParameter> Parameters { get; private set; }

        /// <summary>
        /// 自我标识，用于节点内部的自我识别。
        /// </summary>
        public string SelfAppSystemCode { get; private set; }

        /// <summary>
        /// 时间戳有效期（单位秒）
        /// </summary>
        public int TicksTimeout { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableClientCache { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool EnableOperationLog { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string SqlServerTablesSelect { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string SqlServerTableColumnsSelect { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string SqlServerViewsSelect { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string SqlServerViewColumnsSelect { get; private set; }

        private void Init()
        {
            NameValueCollection values = new NameValueCollection();
            foreach (var item in Parameters.Where(a => string.Equals(a.GroupCode, "Framework", StringComparison.OrdinalIgnoreCase)))
            {
                values.Add(item.Code, item.Value);
            }
            var appSettingsHelper = new AppSettingsHelper(values);
            SelfAppSystemCode = appSettingsHelper.GetString("SelfAppSystemCode");
            TicksTimeout = appSettingsHelper.GetInt32("TicksTimeout");
            EnableClientCache = appSettingsHelper.GetBoolean("EnableClientCache");
            EnableOperationLog = appSettingsHelper.GetBoolean("EnableOperationLog");
            SqlServerTablesSelect = appSettingsHelper.GetString("SqlServerTablesSelect");
            SqlServerTableColumnsSelect = appSettingsHelper.GetString("SqlServerTableColumnsSelect");
            SqlServerViewsSelect = appSettingsHelper.GetString("SqlServerViewsSelect");
            SqlServerViewColumnsSelect = appSettingsHelper.GetString("SqlServerViewColumnsSelect");
        }
    }
}


namespace Anycmd.Host
{
    using Exceptions;
    using Host.AC;
    using Host.AC.Infra;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using Util;

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

        /// <summary>
        /// 命令的InfoID和InfoValue字符串的格式。该格式是命令持久化所使用的默认格式。
        /// </summary>
        public string InfoFormat { get; private set; }

        /// <summary>
        /// 实体库归档路径
        /// </summary>
        public string EntityArchivePath { get; private set; }

        /// <summary>
        /// 实体库备份路径
        /// </summary>
        public string EntityBackupPath { get; private set; }

        /// <summary>
        /// 服务是否可用
        /// </summary>
        public bool ServiceIsAlive { get; private set; }

        /// <summary>
        /// 是否开启Trace
        /// </summary>
        public bool TraceIsEnabled { get; private set; }

        /// <summary>
        /// 检测节点是否在线的请求周期单位（分钟）
        /// </summary>
        public int BeatPeriod { get; private set; }

        /// <summary>
        /// 中心节点标识
        /// </summary>
        public string CenterNodeID { get; private set; }

        /// <summary>
        /// 本节点自我标识
        /// </summary>
        public string ThisNodeID { get; private set; }

        /// <summary>
        /// 审核配置深度
        /// </summary>
        public ConfigLevel AuditLevel { get; private set; }

        /// <summary>
        /// 隐式审核意为
        /// </summary>
        public AuditType ImplicitAudit { get; private set; }

        /// <summary>
        /// 访问控制配置深度
        /// </summary>
        public ConfigLevel ACLLevel { get; private set; }

        /// <summary>
        /// 隐式访问控制意为
        /// </summary>
        public AllowType ImplicitAllow { get; private set; }

        /// <summary>
        /// 实体登录控制配置深度
        /// </summary>
        public ConfigLevel EntityLogonLevel { get; private set; }

        /// <summary>
        /// 隐式实体登录意为
        /// </summary>
        public EntityLogon ImplicitEntityLogon { get; private set; }

        private void Init()
        {
            NameValueCollection values = new NameValueCollection();
            foreach (var item in Parameters)
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

            this.CenterNodeID = appSettingsHelper.GetString("CenterNodeID");
            this.ThisNodeID = appSettingsHelper.GetString("ThisNodeID");
            this.InfoFormat = appSettingsHelper.GetString("InfoFormat");
            this.EntityArchivePath = appSettingsHelper.GetString("EntityArchivePath");
            this.EntityBackupPath = appSettingsHelper.GetString("EntityBackupPath");
            this.ServiceIsAlive = appSettingsHelper.GetBoolean("ServiceIsAlive");
            this.TraceIsEnabled = appSettingsHelper.GetBoolean("TraceIsEnabled");
            this.BeatPeriod = appSettingsHelper.GetInt32("BeatPeriod");

            string auditLevel = appSettingsHelper.GetString("AuditLevel");
            string aclLevel = appSettingsHelper.GetString("ACLLevel");
            string entityLogonLevel = appSettingsHelper.GetString("EntityLogonLevel");
            string implicitAudit = appSettingsHelper.GetString("ImplicitAudit");
            string implicitAllow = appSettingsHelper.GetString("ImplicitAllow");
            string implicitEntityLogon = appSettingsHelper.GetString("ImplicitEntityLogon");
            ConfigLevel _auditLevel, _aclLevel, _entityLogonLevel;
            AuditType _implicitAudit;
            AllowType _implicitAllow;
            EntityLogon _implicitEntityLogon;
            if (!auditLevel.TryParse(out _auditLevel))
            {
                throw new CoreException("审核配置深度配置错误");
            }
            this.AuditLevel = _auditLevel;
            if (!implicitAudit.TryParse(out _implicitAudit))
            {
                throw new CoreException("审核类型配置错误");
            }
            if (_implicitAudit == AuditType.Invalid || _implicitAudit == AuditType.ImplicitAudit)
            {
                throw new CoreException("审核类型配置错误，取值不能是：Invalid或" + AuditType.ImplicitAudit.ToName());
            }
            this.ImplicitAudit = _implicitAudit;

            if (!aclLevel.TryParse(out _aclLevel))
            {
                throw new CoreException("访问控制配置深度配置错误");
            }
            this.ACLLevel = _aclLevel;
            if (!implicitAllow.TryParse(out _implicitAllow))
            {
                throw new CoreException("访问控制类型配置错误");
            }
            if (_implicitAllow == AllowType.Invalid || _implicitAllow == AllowType.ImplicitAllow)
            {
                throw new CoreException("访问控制类型配置错误，取值不能是：Invalid或" + AllowType.ImplicitAllow.ToName());
            }
            this.ImplicitAllow = _implicitAllow;

            if (!entityLogonLevel.TryParse(out _entityLogonLevel))
            {
                throw new CoreException("实体登录控制配置深度配置错误");
            }
            this.EntityLogonLevel = _entityLogonLevel;
            if (!implicitEntityLogon.TryParse(out _implicitEntityLogon))
            {
                throw new CoreException("实体登录控制类型配置错误");
            }
            if (_implicitEntityLogon == EntityLogon.Invalid || _implicitEntityLogon == EntityLogon.ImplicitLogon)
            {
                throw new CoreException("实体登录控制类型配置错误，取值不能是：Invalid或" + EntityLogon.ImplicitLogon.ToName());
            }
            this.ImplicitEntityLogon = _implicitEntityLogon;
        }
    }
}

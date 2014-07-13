
namespace Anycmd.Host.EDI {
    using Exceptions;
    using Host;
    using Host.AC;
    using Host.AC.Infra;
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using Util;

    /// <summary>
    /// Host是什么？Host是提供AnyCommand服务的进程。基础库的进程分四种：Mis进程、WebService进程、执行器进程、分发器进程。
    /// </summary>
    public sealed class HostConfig {
        private static HostConfig instance;

        /// <summary>
        /// 
        /// </summary>
        public static HostConfig Instance {
            get { return instance ?? (instance = NewInstance()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public HostConfig() {
            if (instance == null) return;
            this.Init();
        }

        /// <summary>
        /// 
        /// </summary>
        public static HostConfig ResetInstance() {
            return instance = NewInstance();
        }

        /// <summary>
        /// 
        /// </summary>
        public static HostConfig NewInstance() {
            var config = new HostConfig {
            };
            config.Init();

            return config;
        }

        /// <summary>
        /// 命令的InfoID和InfoValue字符串的格式。该格式是命令持久化所使用的默认格式。
        /// </summary>
        public string InfoFormat { get; set; }

        /// <summary>
        /// 实体库归档路径
        /// </summary>
        public string EntityArchivePath { get; set; }

        /// <summary>
        /// 实体库备份路径
        /// </summary>
        public string EntityBackupPath { get; set; }

        /// <summary>
        /// 服务是否可用
        /// </summary>
        public bool ServiceIsAlive { get; set; }

        /// <summary>
        /// 是否开启Trace
        /// </summary>
        public bool TraceIsEnabled { get; set; }

        /// <summary>
        /// 检测节点是否在线的请求周期单位（分钟）
        /// </summary>
        public int BeatPeriod { get; set; }

        /// <summary>
        /// 中心节点标识
        /// </summary>
        public string CenterNodeID { get; set; }

        /// <summary>
        /// 本节点自我标识
        /// </summary>
        public string ThisNodeID { get; set; }

        /// <summary>
        /// 审核配置深度
        /// </summary>
        public ConfigLevel AuditLevel { get; set; }

        /// <summary>
        /// 隐式审核意为
        /// </summary>
        public AuditType ImplicitAudit { get; set; }

        /// <summary>
        /// 访问控制配置深度
        /// </summary>
        public ConfigLevel ACLLevel { get; set; }

        /// <summary>
        /// 隐式访问控制意为
        /// </summary>
        public AllowType ImplicitAllow { get; set; }

        /// <summary>
        /// 实体登录控制配置深度
        /// </summary>
        public ConfigLevel EntityLogonLevel { get; set; }

        /// <summary>
        /// 隐式实体登录意为
        /// </summary>
        public EntityLogon ImplicitEntityLogon { get; set; }

        private void Init() {
            var values = new NameValueCollection();
            foreach (var item in NodeHost.Instance.AppHost.Config.Parameters.Where(a => "EDICore".Equals(a.GroupCode, StringComparison.OrdinalIgnoreCase)))
            {
                values.Add(item.Code, item.Value);
            }
            var appSettingsHelper = new AppSettingsHelper(values);
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
            if (!auditLevel.TryParse(out _auditLevel)) {
                throw new CoreException("审核配置深度配置错误");
            }
            this.AuditLevel = _auditLevel;
            if (!implicitAudit.TryParse(out _implicitAudit)) {
                throw new CoreException("审核类型配置错误");
            }
            if (_implicitAudit == AuditType.Invalid || _implicitAudit == AuditType.ImplicitAudit) {
                throw new CoreException("审核类型配置错误，取值不能是：Invalid或" + AuditType.ImplicitAudit.ToName());
            }
            this.ImplicitAudit = _implicitAudit;

            if (!aclLevel.TryParse(out _aclLevel)) {
                throw new CoreException("访问控制配置深度配置错误");
            }
            this.ACLLevel = _aclLevel;
            if (!implicitAllow.TryParse(out _implicitAllow)) {
                throw new CoreException("访问控制类型配置错误");
            }
            if (_implicitAllow == AllowType.Invalid || _implicitAllow == AllowType.ImplicitAllow) {
                throw new CoreException("访问控制类型配置错误，取值不能是：Invalid或" + AllowType.ImplicitAllow.ToName());
            }
            this.ImplicitAllow = _implicitAllow;

            if (!entityLogonLevel.TryParse(out _entityLogonLevel)) {
                throw new CoreException("实体登录控制配置深度配置错误");
            }
            this.EntityLogonLevel = _entityLogonLevel;
            if (!implicitEntityLogon.TryParse(out _implicitEntityLogon)) {
                throw new CoreException("实体登录控制类型配置错误");
            }
            if (_implicitEntityLogon == EntityLogon.Invalid || _implicitEntityLogon == EntityLogon.ImplicitLogon) {
                throw new CoreException("实体登录控制类型配置错误，取值不能是：Invalid或" + EntityLogon.ImplicitLogon.ToName());
            }
            this.ImplicitEntityLogon = _implicitEntityLogon;
        }
    }
}

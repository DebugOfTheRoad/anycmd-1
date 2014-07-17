
namespace Anycmd.AC.Identity
{
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// 账户实体基类。
    /// </summary>
    public abstract class AccountBase : EntityBase, IAccount
    {
        private int numberID;
        private string loginName;
        private string password;

        /// <summary>
        /// 说明
        /// </summary>
        public virtual string Description { get; set; }

        public virtual int NumberID
        {
            get { return numberID; }
            set
            {
                if (value == default(int))
                {
                    throw new CoreException("数字标识是必须的");
                }
                else if (numberID != value && numberID != default(int))
                {
                    throw new CoreException("数字标识不能更改");
                }
                numberID = value;
            }
        }

        /// <summary>
        /// 审核状态
        /// </summary>
        public virtual string AuditState { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public virtual string LoginName
        {
            get { return loginName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("登录名不能为空");
                }
                loginName = value;
            }
        }

        public virtual string Password
        {
            get { return password; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ValidationException("密码不能为空");
                }
                password = value;
            }
        }

        /// <summary>
        /// 安全等级
        /// </summary>
        public virtual int? SecurityLevel { get; set; }

        /// <summary>
        /// 修改密码日期
        /// </summary>
        public virtual DateTime? LastPasswordChangeOn { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public virtual string Lang { get; set; }

        /// <summary>
        /// 本体
        /// </summary>
        public virtual string Theme { get; set; }

        /// <summary>
        /// 墙纸
        /// </summary>
        public virtual string Wallpaper { get; set; }

        /// <summary>
        /// 背景色
        /// </summary>
        public virtual string BackColor { get; set; }

        /// <summary>
        /// 允许登录开始时间
        /// </summary>
        public virtual DateTime? AllowStartTime { get; set; }

        /// <summary>
        /// 允许登录结束时间
        /// </summary>
        public virtual DateTime? AllowEndTime { get; set; }

        /// <summary>
        /// 锁定登录开始时间
        /// </summary>
        public virtual DateTime? LockStartTime { get; set; }

        /// <summary>
        /// 锁定登录结束时间
        /// </summary>
        public virtual DateTime? LockEndTime { get; set; }

        /// <summary>
        /// 初次登录时间
        /// </summary>
        public virtual DateTime? FirstLoginOn { get; set; }

        /// <summary>
        /// 上次登录时间
        /// </summary>
        public virtual DateTime? PreviousLoginOn { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public virtual int? LoginCount { get; set; }

        /// <summary>
        /// 开放标识
        /// </summary>
        public virtual string OpenID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public virtual string IPAddress { get; set; }

        /// <summary>
        /// Mac地址
        /// </summary>
        public virtual string MacAddress { get; set; }

        /// <summary>
        /// 密码问题
        /// </summary>
        public virtual string Question { get; set; }

        /// <summary>
        /// 密码问题答案
        /// </summary>
        public virtual string AnswerQuestion { get; set; }

        /// <summary>
        /// 删除标记
        /// </summary>
        public virtual int DeletionStateCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual int IsEnabled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Mobile { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Telephone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string QQ { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string QuickQuery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string QuickQuery1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string QuickQuery2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string OrganizationCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string CommunicationPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string SignedPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string PublicKey { get; set; }
    }
}

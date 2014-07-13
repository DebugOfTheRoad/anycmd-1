
namespace Anycmd.Rdb
{
    using System;

    /// <summary>
    /// SQLServer数据库实体
    /// </summary>
    public sealed class RDatabase : IRDatabase
    {
        private string _catalogName;
        private string _dataSource;
        private string _userID;
        private string _password;
        private string _profile;

        public RDatabase() { }

        /// <summary>
        /// 
        /// </summary>
        public bool IsTemplate { get; set; }

        /// <summary>
        /// 数据库标识
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// 类别码
        /// </summary>
        public string RdbmsType { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string CatalogName
        {
            get { return _catalogName; }
            set
            {
                if (value != _catalogName)
                {
                    _catalogName = value;
                }
            }
        }

        /// <summary>
        /// 数据源
        /// </summary>
        public string DataSource
        {
            get { return _dataSource; }
            set
            {
                if (value != _dataSource)
                {
                    _dataSource = value;
                }
            }
        }

        /// <summary>
        /// 数据库连接用户名
        /// </summary>
        public string UserID
        {
            get { return _userID; }
            set
            {
                if (value != _userID)
                {
                    _userID = value;
                }
            }
        }

        /// <summary>
        /// 数据库连接用户密码
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                if (value != _password)
                {
                    _password = value;
                }
            }
        }

        /// <summary>
        /// 数据库连接属性
        /// </summary>
        public string Profile
        {
            get { return _profile; }
            set
            {
                if (value != _profile)
                {
                    _profile = value;
                }
            }
        }

        /// <summary>
        /// 数据访问提供程序名称
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        public string CreateBy { get; set; }

        public DateTime? CreateOn { get; set; }

        public Guid? CreateUserID { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public Guid? ModifiedUserID { get; set; }
    }
}

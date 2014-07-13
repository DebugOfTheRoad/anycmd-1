
namespace Anycmd.EDI {
    using Exceptions;
    using Model;
    using System;

    /// <summary>
    /// <see cref="IArchive"/>
    /// </summary>
    public abstract class ArchiveBase : EntityBase, IArchive {
        private int _numberID;
        private Guid _ontologyID;

        public string RdbmsType { get; set; }

        /// <summary>
        /// 源
        /// </summary>
        public string DataSource { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 数字标识
        /// </summary>
        public int NumberID {
            get { return _numberID; }
            set {
                if (value != _numberID) {
                    if (_numberID != default(int))
                    {
                        throw new ValidationException("数字标识不能更改");
                    }
                    _numberID = value;   
                }
            }
        }
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 本体
        /// </summary>
        public Guid OntologyID {
            get { return _ontologyID; }
            set {
                if (value != _ontologyID) {
                    if (_ontologyID != Guid.Empty) {
                        throw new CoreException("不能更改所属本体");
                    }
                    _ontologyID = value;
                }
            }
        }
        /// <summary>
        /// 归档时间
        /// </summary>
        public DateTime ArchiveOn { get; set; }
    }
}

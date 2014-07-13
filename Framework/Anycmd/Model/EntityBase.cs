
namespace Anycmd.Model
{
    using Exceptions;
    using Extensions;
    using System;

    /// <summary>
    /// 实体模型基类型
    /// <remarks>
    /// 在EntityObject的基础上增加了CreateOn、CreateUserID、
    /// CreateBy、ModifiedOn、ModifiedUserID、ModifiedBy六个属性
    /// </remarks>
    /// </summary>
    public abstract class EntityBase : EntityObject, IEntityBase
    {
        private DateTime? createOn;
        private DateTime? modifiedOn;
        private Guid? createUserID;
        private Guid? modifiedUserID;
        private string createBy;
        private string modifiedBy;

        public DateTime? CreateOn
        {
            get { return createOn; }
            protected set
            {
                if (createOn.HasValue && createOn != value)
                {
                    throw new ValidationException("创建时间不能更改");
                }
                createOn = value;
            }
        }

        public Guid? CreateUserID
        {
            get { return createUserID; }
            protected set
            {
                if (createUserID.HasValue && createUserID != value)
                {
                    throw new ValidationException("创建人不能更改");
                }
                createUserID = value;
            }
        }

        public string CreateBy
        {
            get { return createBy; }
            protected set { createBy = value; }
        }

        public DateTime? ModifiedOn
        {
            get { return modifiedOn; }
            protected set
            {
                if (value != null)
                {
                    if (value.Value.IsNotValid())
                    {
                        throw new ValidationException("ModifiedOn值不合法" + value);
                    }
                }
                modifiedOn = value;
            }
        }

        public Guid? ModifiedUserID
        {
            get { return modifiedUserID; }
            protected set
            {
                if (value != null)
                {
                    if (modifiedUserID != null && value.Value == Guid.Empty)
                    {
                        throw new ValidationException("最后修改人标识错误" + value);
                    }
                }
                modifiedUserID = value;
            }
        }

        public string ModifiedBy
        {
            get { return modifiedBy; }
            protected set { modifiedBy = value; }
        }

        #region 显示实现
        /// <summary>
        /// 创建时间
        /// </summary>
        DateTime? IEntityBase.CreateOn
        {
            get { return createOn; }
            set
            {
                this.CreateOn = value;
            }
        }

        /// <summary>
        /// 创建人标识
        /// </summary>
        Guid? IEntityBase.CreateUserID
        {
            get { return createUserID; }
            set
            {
                this.CreateUserID = value;
            }
        }

        /// <summary>
        /// 创建人[姓名|登录名]
        /// </summary>
        string IEntityBase.CreateBy
        {
            get { return createBy; }
            set { this.CreateBy = value; }
        }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        DateTime? IEntityBase.ModifiedOn
        {
            get { return modifiedOn; }
            set
            {
                this.ModifiedOn = value;
            }
        }

        /// <summary>
        /// 最后修改人标识
        /// </summary>
        Guid? IEntityBase.ModifiedUserID
        {
            get { return modifiedUserID; }
            set
            {
                this.ModifiedUserID = value;
            }
        }

        /// <summary>
        /// 最后修改人[姓名|登录名]
        /// </summary>
        string IEntityBase.ModifiedBy
        {
            get { return modifiedBy; }
            set { this.ModifiedBy = value; }
        }
        #endregion
    }
}

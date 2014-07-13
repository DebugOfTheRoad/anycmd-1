
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using Anycmd.Rdb;
    using Exceptions;
    using System;
    using System.Reflection;
    using Util;

    public sealed class PropertyState : IProperty
    {
        private Guid _entityTypeID;
        private Guid? _dicID;
        private PropertyInfo _propertyInfo;
        private bool _propertyInfoed = false;
        private EntityTypeState entityType = EntityTypeState.Empty;

        private PropertyState() { }

        #region 工厂方法
        public static PropertyState Create(AppHost host, PropertyBase property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            if (property.EntityTypeID == Guid.Empty)
            {
                throw new CoreException("实体属性必须属于某个实体类型");
            }
            EntityTypeState entityType;
            if (!host.EntityTypeSet.TryGetEntityType(property.EntityTypeID, out entityType))
            {
                throw new CoreException("意外的实体类型标识" + property.EntityTypeID);
            }
            Guid? dicID = property.DicID;
            if (dicID == Guid.Empty)
            {
                dicID = null;
            }
            if (dicID.HasValue)
            {
                if (!host.DicSet.ContainsDic(dicID.Value))
                {
                    throw new ValidationException("意外的字典标识" + dicID);
                }
            }
            return new PropertyState
            {
                AppHost = host,
                EntityTypeID = property.EntityTypeID,
                ForeignPropertyID = property.ForeignPropertyID,
                Code = property.Code,
                CreateOn = property.CreateOn,
                DicID = dicID,
                GuideWords = property.GuideWords,
                Icon = property.Icon,
                Id = property.Id,
                InputType = property.InputType,
                IsDetailsShow = property.IsDetailsShow,
                MaxLength = property.MaxLength,
                Name = property.Name,
                SortCode = property.SortCode,
                Tooltip = property.Tooltip,
                IsTotalLine = property.IsTotalLine,
                IsDeveloperOnly = property.IsDeveloperOnly,
                IsInput = property.IsInput
            };
        }

        public static PropertyState CreateNoneProperty(string code)
        {
            return new PropertyState
            {
                _entityTypeID = EntityTypeState.Empty.Id,
                ForeignPropertyID = null,
                Code = code,
                CreateOn = SystemTime.MinDate,
                _dicID = Guid.Empty,
                GuideWords = "警告：编码为" + code + "的字段不存在",
                Icon = string.Empty,
                Id = Guid.Empty,
                InputType = string.Empty,
                IsDetailsShow = false,
                IsInput = false,
                MaxLength = 0,
                Name = code,
                SortCode = 0,
                Tooltip = "不存在的字段",
                IsTotalLine = false,
                IsDeveloperOnly = false
            };
        }
        #endregion

        public AppHost AppHost { get; private set; }

        public Guid Id { get; private set; }

        public Guid EntityTypeID
        {
            get { return _entityTypeID; }
            private set
            {
                EntityTypeState entityType;
                if (!AppHost.EntityTypeSet.TryGetEntityType(value, out entityType))
                {
                    throw new ValidationException("意外的实体属性实体类型标识" + value);
                }
                _entityTypeID = value;
            }
        }

        public Guid? ForeignPropertyID { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public string GuideWords { get; private set; }

        public string Tooltip { get; set; }

        public int? MaxLength { get; private set; }

        public int SortCode { get; private set; }

        public Guid? DicID
        {
            get { return _dicID; }
            private set
            {
                if (value == Guid.Empty)
                {
                    value = null;
                }
                if (value.HasValue)
                {
                    if (!AppHost.DicSet.ContainsDic(value.Value))
                    {
                        throw new ValidationException("意外的实体属性字典标识" + value);
                    }
                }
                _dicID = value;
            }
        }

        public string Icon { get; private set; }

        public bool IsDetailsShow { get; private set; }

        public bool IsDeveloperOnly { get; private set; }

        public bool IsInput { get; private set; }

        public string InputType { get; private set; }

        public bool IsTotalLine { get; private set; }

        public bool IsViewField
        {
            get
            {
                return PropertyInfo == null;
            }
        }

        public DateTime? CreateOn { get; private set; }

        public PropertyInfo PropertyInfo
        {
            get
            {
                if (_propertyInfoed)
                {
                    return _propertyInfo;
                }
                _propertyInfo = this.EntityType.Map.ClrType.GetProperty(this.Code);
                _propertyInfoed = true;
                return _propertyInfo;
            }
        }

        private EntityTypeState EntityType
        {
            get
            {
                if (entityType == EntityTypeState.Empty)
                {
                    if (!AppHost.EntityTypeSet.TryGetEntityType(this.EntityTypeID, out entityType))
                    {
                        throw new CoreException("意外的实体类型标识" + this.EntityTypeID);
                    }
                }
                return entityType;
            }
        }

        private RdbDescriptor _database;
        private RdbDescriptor Database
        {
            get
            {
                if (_database == null)
                {
                    if (!AppHost.Rdbs.TryDb(EntityType.DatabaseID, out _database))
                    {
                        throw new CoreException("意外的数据库标识" + EntityType.DatabaseID);
                    }
                }
                return _database;
            }
        }

        private DbTableColumn _tableColumn;
        private DbTableColumn TableColumn
        {
            get
            {
                if (_tableColumn == null)
                {
                    if (string.IsNullOrEmpty(EntityType.TableName))
                    {
                        return null;
                    }
                    AppHost.DbTableColumns.TryGetDbTableColumn(Database, string.Format("[{0}][{1}][{2}]", EntityType.SchemaName, EntityType.TableName, this.Code), out _tableColumn);
                }

                return _tableColumn;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConfigValid
        {
            get
            {
                if (this.IsViewField)
                {
                    return true;
                }
                bool isValid = true;
                if (PropertyInfo != null && !PropertyInfo.Name.Equals(this.Code, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
                if (!string.IsNullOrEmpty(EntityType.TableName))
                {
                    if (TableColumn == null)
                    {
                        isValid = false;
                    }
                    else if (TableColumn.MaxLength.HasValue && TableColumn.MaxLength > 0 && this.MaxLength > TableColumn.MaxLength)
                    {
                        isValid = false;
                    }
                }

                return isValid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DbIsNullable
        {
            get
            {
                if (TableColumn == null)
                {
                    return false;
                }
                return TableColumn.IsNullable;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string DbTypeName
        {
            get
            {
                if (TableColumn == null)
                {
                    return string.Empty;
                }
                return TableColumn.TypeName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? DbMaxLength
        {
            get
            {
                if (TableColumn == null)
                {
                    return null;
                }
                return TableColumn.MaxLength;
            }
        }

        #region override Equals and GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!(obj is PropertyState))
            {
                return false;
            }
            var left = this;
            var right = (PropertyState)obj;

            return left.Id == right.Id &&
                left.EntityTypeID == right.EntityTypeID &&
                left.ForeignPropertyID == right.ForeignPropertyID &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.GuideWords == right.GuideWords &&
                left.Tooltip == right.Tooltip &&
                left.MaxLength == right.MaxLength &&
                left.PropertyInfo == right.PropertyInfo &&
                left.SortCode == right.SortCode &&
                left.DicID == right.DicID &&
                left.Icon == right.Icon &&
                left.IsDetailsShow == right.IsDetailsShow &&
                left.IsDeveloperOnly == right.IsDeveloperOnly &&
                left.InputType == right.InputType && 
                left.IsInput == right.IsInput &&
                left.IsTotalLine == right.IsTotalLine &&
                left.IsViewField == right.IsViewField;
        }

        public static bool operator ==(PropertyState a, PropertyState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(PropertyState a, PropertyState b)
        {
            return !(a == b);
        }
        #endregion
    }
}

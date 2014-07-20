
namespace Anycmd.AC.Infra.ViewModels.EntityTypeViewModels
{
    using Anycmd.Host;
    using Exceptions;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public partial class PropertyTr
    {
        private readonly IAppHost host;

        private PropertyTr(IAppHost host)
        {
            this.host = host;
        }

        public static PropertyTr Create(PropertyState property)
        {
            if (property == null)
            {
                return null;
            }
            string clrPropertyType = string.Empty;
            string clrPropertyName = string.Empty;
            if (property.PropertyInfo != null)
            {
                clrPropertyType = property.PropertyInfo.PropertyType.Name;
                if (clrPropertyType == typeof(Nullable<>).Name)
                {
                    clrPropertyType = property.PropertyInfo.PropertyType.GetGenericArguments()[0].Name + "?";
                }
                clrPropertyName = property.PropertyInfo.Name;
            }
            return new PropertyTr(property.AppHost)
            {
                Code = property.Code,
                CreateOn = property.CreateOn,
                DicID = property.DicID,
                EntityTypeID = property.EntityTypeID,
                Icon = property.Icon,
                Id = property.Id,
                InputType = property.InputType,
                IsDetailsShow = property.IsDetailsShow,
                IsDeveloperOnly = property.IsDeveloperOnly,
                IsViewField = property.IsViewField,
                MaxLength = property.MaxLength,
                Name = property.Name,
                SortCode = property.SortCode,
                IsConfigValid = property.IsConfigValid,
                DbIsNullable = property.DbIsNullable,
                DbMaxLength = property.DbMaxLength,
                DbTypeName = property.DbTypeName,
                ClrPropertyType = clrPropertyType,
                ClrPropertyName = clrPropertyName
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid EntityTypeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int? MaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid? DicID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string DicName
        {
            get
            {
                if (!this.DicID.HasValue)
                {
                    return string.Empty;
                }
                DicState dic;
                if (!host.DicSet.TryGetDic(this.DicID.Value, out dic))
                {
                    throw new CoreException("意外的系统字典标识" + this.DicID);
                }
                return dic.Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsDetailsShow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsDeveloperOnly { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsViewField { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string InputType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsConfigValid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool DbIsNullable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DbTypeName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? DbMaxLength { get; set; }

        public string ClrPropertyType { get; set; }

        public string ClrPropertyName { get; set; }
    }
}

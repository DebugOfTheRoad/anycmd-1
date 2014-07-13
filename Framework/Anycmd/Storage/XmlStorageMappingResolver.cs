
using Anycmd.Serialization;
using System;
using System.IO;
using System.Linq;

namespace Anycmd.Storage
{
    /// <summary>
    /// Represents the XML storage mapping resolver.
    /// </summary>
    public class XmlStorageMappingResolver : IStorageMappingResolver
    {
        #region Private Fields
        private readonly string fileName;
        private readonly IObjectSerializer serializer = new ObjectXmlSerializer();
        private readonly StorageMappingSchema mappingSchema;
        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of <c>XmlStorageMappingResolver</c> class.
        /// </summary>
        /// <param name="fileName">The file name of the external XML mapping file.</param>
        public XmlStorageMappingResolver(string fileName)
        {
            this.fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            using (FileStream fileStream = new FileStream(this.fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, Convert.ToInt32(fileStream.Length));
                mappingSchema = serializer.Deserialize<StorageMappingSchema>(bytes);
                fileStream.Close();
            }
        }
        #endregion

        #region Private Methods
        private bool ValidateSchema()
        {
            if (mappingSchema != null &&
                mappingSchema.DataTypes != null &&
                mappingSchema.DataTypes.DataType != null &&
                mappingSchema.DataTypes.DataType.Length > 0)
                return true;
            return false;
        }
        #endregion

        #region IStorageMappingResolver Members
        /// <summary>
        /// Resolves the table name by using the given type.
        /// </summary>
        /// <typeparam name="T">The type of the object to be resolved.</typeparam>
        /// <returns>The table name.</returns>
        public string ResolveTableName<T>() where T : class, new()
        {
            if (ValidateSchema())
            {
                var dataType = mappingSchema.DataTypes.DataType.FirstOrDefault(p => p.FullName.Equals(typeof(T).FullName));
                if (dataType != null && !string.IsNullOrEmpty(dataType.MapTo))
                    return dataType.MapTo;
                else
                    return typeof(T).Name;
            }
            else
                return typeof(T).Name;
        }
        /// <summary>
        /// Resolves the field name by using the given type and property name.
        /// </summary>
        /// <typeparam name="T">The type of the object to be resolved.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The field name.</returns>
        public string ResolveFieldName<T>(string propertyName) where T : class, new()
        {
            if (ValidateSchema())
            {
                var dataType = mappingSchema.DataTypes.DataType.FirstOrDefault(p => p.FullName.Equals(typeof(T).FullName));
                if (dataType != null)
                {
                    if (dataType.Properties != null && dataType.Properties.Property != null && dataType.Properties.Property.Length > 0)
                    {
                        var property = dataType.Properties.Property.FirstOrDefault(p => p.Name.Equals(propertyName));
                        if (property != null && !string.IsNullOrEmpty(property.MapTo))
                            return property.MapTo;
                        else
                            return propertyName;
                    }
                    else
                        return propertyName;
                }
                else
                    return propertyName;
            }
            else
                return propertyName;
        }
        /// <summary>
        /// Checks if the given property is mapped to an auto-generated identity field.
        /// </summary>
        /// <typeparam name="T">The type of the object to be resolved.</typeparam>
        /// <param name="propertyName">The property name.</param>
        /// <returns>True if the field is mapped to an auto-generated identity, otherwise false.</returns>
        public bool IsAutoIdentityField<T>(string propertyName) where T : class, new()
        {
            if (ValidateSchema())
            {
                var dataType = mappingSchema.DataTypes.DataType.FirstOrDefault(p => p.FullName.Equals(typeof(T).FullName));
                if (dataType != null)
                {
                    if (dataType.Properties != null && dataType.Properties.Property != null && dataType.Properties.Property.Length > 0)
                    {
                        var property = dataType.Properties.Property.FirstOrDefault(p => p.Name.Equals(propertyName));
                        if (property != null)
                            return property.AutoGenerate;
                        else
                            return false;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else
                return false;
        }
        #endregion
    }
}

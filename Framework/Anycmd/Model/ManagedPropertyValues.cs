
namespace Anycmd.Model
{
    using Anycmd.Host;
    using Exceptions;
    using System.Collections.Generic;
    using System.Reflection;

    // TODO:考察访问者模式，看看是否是访问者模式
    public abstract class ManagedPropertyValues : IManagedPropertyValues
    {
        public IEnumerable<ManagedPropertyValue> GetValues(EntityTypeState entityType)
        {
            var properties = this.GetType().GetProperties(BindingFlags.Public & BindingFlags.SetProperty);
            foreach (var property in properties)
            {
                yield return new ManagedPropertyValue(GetProperty(entityType, property.Name), property.GetValue(this));
            }
        }

        private static PropertyState GetProperty(EntityTypeState entityType, string propertyCode)
        {
            if (propertyCode.Equals(entityType.Code + "ID", System.StringComparison.OrdinalIgnoreCase))
            {
                propertyCode = "Id";
            }
            PropertyState property;
            if (!entityType.AppHost.EntityTypeSet.TryGetProperty(entityType, propertyCode, out property))
            {
                throw new CoreException("意外的" + entityType.Name + "实体属性编码" + propertyCode);
            }
            return property;
        }
    }
}

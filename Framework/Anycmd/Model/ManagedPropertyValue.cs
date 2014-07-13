using Anycmd.Host;
using System;

namespace Anycmd.Model
{
    public class ManagedPropertyValue
    {
        public ManagedPropertyValue(PropertyState property, object value)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            this.Property = property;
            this.Value = value;
        }

        public PropertyState Property { get; private set; }

        public object Value { get; private set; }
    }
}


namespace Anycmd.Model
{
    using Anycmd.Host;

    /// <summary>
    /// 托管属性值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ManagedPropertyValue<T> : ManagedPropertyValue
    {
        public ManagedPropertyValue(PropertyState property, T value)
            : base(property, value)
        {
            this.Value = value;
        }

        public new T Value { get; private set; }
    }
}

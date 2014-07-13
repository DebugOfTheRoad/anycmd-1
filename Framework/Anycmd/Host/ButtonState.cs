
namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using System;

    public sealed class ButtonState : IButton
    {
        private ButtonState() { }

        public static ButtonState Create(ButtonBase button)
        {
            if (button == null)
            {
                throw new ArgumentNullException("button");
            }
            return new ButtonState
            {
                Id = button.Id,
                Name = button.Name,
                CategoryCode = button.CategoryCode,
                Code = button.Code,
                Icon = button.Icon,
                SortCode = button.SortCode,
                IsEnabled = button.IsEnabled,
                CreateOn = button.CreateOn
            };
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Code { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string CategoryCode { get; set; }

        public string Icon { get; private set; }

        public int SortCode { get; private set; }

        public int IsEnabled { get; private set; }

        public DateTime? CreateOn { get; private set; }

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
            if (!(obj is ButtonState))
            {
                return false;
            }
            var left = this;
            var right = (ButtonState)obj;

            return left.Id == right.Id &&
                left.Name == right.Name &&
                left.Code == right.Code &&
                left.CategoryCode == right.CategoryCode &&
                left.Icon == right.Icon &&
                left.SortCode == right.SortCode &&
                left.IsEnabled == right.IsEnabled;
        }

        public static bool operator ==(ButtonState a, ButtonState b)
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

        public static bool operator !=(ButtonState a, ButtonState b)
        {
            return !(a == b);
        }
    }
}

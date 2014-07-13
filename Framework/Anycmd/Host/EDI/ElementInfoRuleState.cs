using Anycmd.EDI;
using System;

namespace Anycmd.Host.EDI
{
    public sealed class ElementInfoRuleState : IElementInfoRule
    {
        private ElementInfoRuleState() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementInfoRule"></param>
        /// <returns></returns>
        public static ElementInfoRuleState Create(IElementInfoRule elementInfoRule)
        {
            InfoRuleState infoRule;
            if (!NodeHost.Instance.InfoRules.TryGetInfoRule(elementInfoRule.InfoRuleID, out infoRule))
            {
                throw new InvalidProgramException("请检测InfoRule的存在性");
            }
            return new ElementInfoRuleState
            {
                CreateOn = elementInfoRule.CreateOn,
                ElementID = elementInfoRule.ElementID,
                Id = elementInfoRule.Id,
                InfoRuleID = elementInfoRule.InfoRuleID,
                IsEnabled = elementInfoRule.IsEnabled,
                SortCode = elementInfoRule.SortCode
            };
        }

        public Guid Id { get; private set; }

        public Guid ElementID { get; private set; }

        public Guid InfoRuleID { get; private set; }

        public int IsEnabled { get; private set; }

        public int SortCode { get; private set; }

        public DateTime CreateOn { get; private set; }

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
            if (!(obj is ElementInfoRuleState))
            {
                return false;
            }
            var left = this;
            var right = (ElementInfoRuleState)obj;

            return
                left.Id == right.Id &&
                left.ElementID == right.ElementID &&
                left.InfoRuleID == right.InfoRuleID &&
                left.IsEnabled == right.IsEnabled &&
                left.SortCode == right.SortCode;
        }

        public static bool operator ==(ElementInfoRuleState a, ElementInfoRuleState b)
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

        public static bool operator !=(ElementInfoRuleState a, ElementInfoRuleState b)
        {
            return !(a == b);
        }
    }
}

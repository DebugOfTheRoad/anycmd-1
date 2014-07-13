using System;

namespace Anycmd.Host.EDI
{
    using Info;

    public sealed class InfoRuleState
    {
        private InfoRuleState() { }

        public static InfoRuleState Create(InfoRuleEntityBase entity, IInfoRule infoRule)
        {
            return new InfoRuleState
            {
                Id = entity.Id,
                CreateOn = entity.CreateOn,
                IsEnabled = entity.IsEnabled,
                InfoRule = infoRule
            };
        }

        /// <summary>
        /// 验证器标识
        /// </summary>
        public Guid Id { get; private set; }

        public int IsEnabled { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public IInfoRule InfoRule { get; private set; }
    }
}

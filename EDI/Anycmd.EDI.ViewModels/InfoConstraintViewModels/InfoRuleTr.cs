
namespace Anycmd.EDI.ViewModels.InfoConstraintViewModels
{
    using Anycmd.Host.EDI;
    using System;
    using System.Collections.Generic;

    // TODO:使用InfoRuleState来组合IInfoRule和CreateOn和IsEnabled
    /// <summary>
    /// 
    /// </summary>
    public class InfoRuleTr
    {
        private InfoRuleTr() { }

        public static InfoRuleTr Create(AppHost host, InfoRuleState infoRule)
        {
            return new InfoRuleTr
            {
                Id = infoRule.Id,
                Author = infoRule.InfoRule.Author,
                Description = infoRule.InfoRule.Description,
                FullName = infoRule.InfoRule.GetType().Name,
                Name = infoRule.InfoRule.Name,
                Title = infoRule.InfoRule.Title,
                CreateOn = infoRule.CreateOn,
                IsEnabled = infoRule.IsEnabled
            };
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }

        public int IsEnabled { get; set; }

        public DateTime? CreateOn { get; set; }
    }
}

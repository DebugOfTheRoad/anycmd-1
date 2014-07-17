
namespace Anycmd.EDI.InfoConstraints
{
    using Host.EDI;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IInfoRule))]
    public class YearMonthDayInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "16BC2B76-78BF-47CC-B886-EFE4C552EA46";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "年月日（YYYYMMDD）验证器";
        private static readonly string description = "年月日（YYYYMMDD）验证器";
        private static readonly string author = "xuexs";

        public YearMonthDayInfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            return ProcessResult.Ok;
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}

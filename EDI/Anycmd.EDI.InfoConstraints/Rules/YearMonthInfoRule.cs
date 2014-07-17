
namespace Anycmd.EDI.InfoConstraints
{
    using Host.EDI;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IInfoRule))]
    public class YearMonthInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "3C300FC9-7E81-4425-8EE2-C4355DD08502";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "年月（YYYYMM）验证器";
        private static readonly string description = "年月（YYYYMM）验证器";
        private static readonly string author = "xuexs";

        public YearMonthInfoCheck()
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


namespace Anycmd.EDI.InfoConstraints
{
    using Host.EDI;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IInfoRule))]
    public class YearInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "A4385990-D9FA-4ECD-8287-A3C41F72209E";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "年（YYYY）验证器";
        private static readonly string description = "年（YYYY）验证器";
        private static readonly string author = "xuexs";

        public YearInfoCheck()
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

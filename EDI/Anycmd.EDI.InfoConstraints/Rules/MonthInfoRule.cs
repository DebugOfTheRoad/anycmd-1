
namespace Anycmd.EDI.InfoConstraints
{
    using Anycmd.Host.EDI;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IInfoRule))]
    public class MonthInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "8B3A5C1B-B723-4A30-9F62-BB4484287B3E";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "月（MM）验证器";
        private static readonly string description = "月（MM）验证器";
        private static readonly string author = "xuexs";

        public MonthInfoCheck()
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


namespace Anycmd.EDI.InfoConstraints
{
    using Host.EDI;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IInfoRule))]
    public class Int64InfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "8040267B-F931-4009-A959-62BF344EF1EB";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "Int64验证器";
        private static readonly string description = "Int64验证器";
        private static readonly string author = "xuexs";

        public Int64InfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            try
            {
                bool isValid = true;
                Status stateCode = Status.Ok;
                string description = "Int64验证通过";
                Guid guid;
                if (!string.IsNullOrEmpty(value) && !Guid.TryParse(value, out guid))
                {
                    description = "非法的Int64" + value;
                    stateCode = Status.InvalidInfoValue;
                    isValid = false;
                }

                return new ProcessResult(isValid, stateCode, description);
            }
            catch (Exception ex)
            {
                return new ProcessResult(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}

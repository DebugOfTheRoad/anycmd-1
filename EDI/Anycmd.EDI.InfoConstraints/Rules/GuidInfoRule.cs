
namespace Anycmd.EDI.InfoConstraints
{
    using Host.EDI;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IInfoRule))]
    public class GuidInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "A93559F9-5268-4225-92CA-2D394FE9A8B3";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "Guid验证器";
        private static readonly string description = "Guid验证器";
        private static readonly string author = "xuexs";

        public GuidInfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            try
            {
                bool isValid = true;
                Status stateCode = Status.Ok;
                string description = "Guid验证通过";
                Guid guid;
                if (!string.IsNullOrEmpty(value) && !Guid.TryParse(value, out guid))
                {
                    description = "非法的Guid" + value;
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

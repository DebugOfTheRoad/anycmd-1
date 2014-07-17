
namespace Anycmd.EDI.InfoConstraints
{
    using Host.EDI;
    using System;
    using System.ComponentModel.Composition;

    [Export(typeof(IInfoRule))]
    public class Int32InfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "9E7522F9-11B8-49D1-9803-D755A2A27530";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "Int32验证器";
        private static readonly string description = "Int32验证器";
        private static readonly string author = "xuexs";

        public Int32InfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            try
            {
                bool isValid = true;
                Status stateCode = Status.Ok;
                string description = "Int32验证通过";
                Guid guid;
                if (!string.IsNullOrEmpty(value) && !Guid.TryParse(value, out guid))
                {
                    description = "非法的Int32" + value;
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

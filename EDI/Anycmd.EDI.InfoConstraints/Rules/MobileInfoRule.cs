
namespace Anycmd.EDI.InfoConstraints
{
    using Anycmd.Host.EDI;
    using System;
    using System.ComponentModel.Composition;
    using System.Text.RegularExpressions;

    [Export(typeof(IInfoRule))]
    public sealed class MobileInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "85036E88-D35A-42AA-B631-54221E5561C4";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "手机号码验证器";
        private static readonly string description = "使用正则表达式验证手机号码格式的合法性";
        private static readonly string author = "xuexs";
        private static readonly Regex emailExpression = new Regex(@"^0?(13[0-9]|15[012356789]|18[0-9]|14[57])[0-9]{8}$", RegexOptions.Singleline | RegexOptions.Compiled);

        public MobileInfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            try
            {
                bool isValid = true;
                Status stateCode = Status.Ok;
                string description = "手机号码验证通过";
                if (!string.IsNullOrEmpty(value) && !emailExpression.IsMatch(value))
                {
                    description = "非法的手机号码" + value;
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


namespace Anycmd.EDI.InfoConstraints
{
    using Host.EDI;
    using System;
    using System.ComponentModel.Composition;
    using System.Text.RegularExpressions;

    [Export(typeof(IInfoRule))]
    public class ZipCodeInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "2B237AC1-3779-4637-95E7-6AA419EFC2C8";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "邮政编码验证器";
        private static readonly string description = "使用正则表达式验证邮政编码格式的合法性";
        private static readonly string author = "xuexs";
        private static readonly Regex emailExpression = new Regex(@"^[1-9][0-9]{5}$", RegexOptions.Singleline | RegexOptions.Compiled);

        public ZipCodeInfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            try
            {
                bool isValid = true;
                Status stateCode = Status.Ok;
                string description = "邮政编码验证通过";
                isValid = !string.IsNullOrEmpty(value) && emailExpression.IsMatch(value);
                if (!isValid)
                {
                    description = "非法的邮政编码";
                    stateCode = Status.InvalidInfoValue;
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

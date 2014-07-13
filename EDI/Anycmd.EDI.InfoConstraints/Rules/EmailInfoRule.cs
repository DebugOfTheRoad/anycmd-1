
namespace Anycmd.EDI.InfoConstraints
{
    using Anycmd.Host.EDI;
    using System;
    using System.ComponentModel.Composition;
    using System.Text.RegularExpressions;

    [Export(typeof(IInfoRule))]
    public sealed class EmailInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "44780236-BFDD-47DE-A8CD-1BF51ADDB576";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "电子邮箱（Email）验证器";
        private static readonly string description = "使用正则表达式验证邮箱格式的合法性";
        private static readonly string author = "xuexs";
        private static readonly Regex emailExpression = new Regex(
            @"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$"
            , RegexOptions.Singleline | RegexOptions.Compiled);

        public EmailInfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            try
            {
                bool isValid = true;
                Status stateCode = Status.Ok;
                string description = "电子邮箱验证通过";
                isValid = !string.IsNullOrEmpty(value) && emailExpression.IsMatch(value);
                if (!isValid)
                {
                    description = "非法的电子邮箱地址";
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

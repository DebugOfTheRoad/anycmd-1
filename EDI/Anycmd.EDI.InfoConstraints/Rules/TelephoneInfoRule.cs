﻿
namespace Anycmd.EDI.InfoConstraints
{
    using Anycmd.Host.EDI;
    using System;
    using System.ComponentModel.Composition;
    using System.Text.RegularExpressions;

    [Export(typeof(IInfoRule))]
    public sealed class TelephoneInfoCheck : InfoRuleBase, IInfoRule
    {
        private static readonly string actorID = "88F8EDE3-BA84-45D1-9D5B-1ACD682381F5";
        private static readonly Guid id = new Guid(actorID);
        private static readonly string title = "固定电话号码验证器";
        private static readonly string description = "使用正则表达式验证固定电话号码格式的合法性";
        private static readonly string author = "xuexs";
        private static readonly Regex emailExpression = new Regex(@"(\d{4}-|\d{3}-)?(\d{8}|\d{7})", RegexOptions.Singleline | RegexOptions.Compiled);

        public TelephoneInfoCheck()
            : base(id, title, author, description)
        {

        }

        public ProcessResult Valid(string value)
        {
            try
            {
                bool isValid = true;
                Status stateCode = Status.Ok;
                string description = "固定电话验证通过";
                isValid = !string.IsNullOrEmpty(value) && emailExpression.IsMatch(value);
                if (!isValid)
                {
                    description = "非法的固定电话号码";
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

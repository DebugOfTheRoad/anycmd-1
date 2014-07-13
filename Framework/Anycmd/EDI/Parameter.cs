
namespace Anycmd.EDI {
    using Host;
    using System;

    public sealed class Parameter : ParameterBase {
        private Parameter() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupCode"></param>
        /// <param name="categoryCode"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal Parameter(
            Guid id, string groupCode, string categoryCode, 
            string code, string name, string value) {
            this.Id = id;
            this.GroupCode = groupCode;
            this.CategoryCode = categoryCode;
            this.Code = code;
            this.Name = name;
            this.Value = value;
        }
    }
}

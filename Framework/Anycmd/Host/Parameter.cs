
namespace Anycmd.Host
{
    using Extensions;
    using System;
    using System.Data;

    /// <summary>
    /// 系统参数模型
    /// </summary>
    public sealed class Parameter : ParameterBase
    {
        public Parameter() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="groupCode"></param>
        /// <param name="categoryCode"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal Parameter(Guid id, string groupCode, string categoryCode, string code, string name, string value)
        {
            this.Id = id;
            this.GroupCode = groupCode;
            this.CategoryCode = categoryCode;
            this.Code = code;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="record"></param>
        internal Parameter(IDataRecord record)
            : this(record.GetGuid(record.GetOrdinal("Id")),
            record.GetString(record.GetOrdinal("GroupCode")),
            record.GetString(record.GetOrdinal("CategoryCode")),
            record.GetString(record.GetOrdinal("Code")),
            record.GetString(record.GetOrdinal("Name")),
            record.GetString(record.GetOrdinal("Value")))
        {
            this.SortCode = record.GetInt32(record.GetOrdinal("SortCode"));
            this.CreateBy = record.GetNullableString("CreateBy");
            this.CreateOn = record.GetNullableDateTime("CreateOn");
            this.CreateUserID = record.GetNullableGuid("CreateUserID");
            this.ModifiedBy = record.GetNullableString("ModifiedBy");
            this.ModifiedOn = record.GetNullableDateTime("ModifiedOn");
            this.ModifiedUserID = record.GetNullableGuid("ModifiedUserID");
            this.Description = record.GetNullableString("Description");
        }
    }
}

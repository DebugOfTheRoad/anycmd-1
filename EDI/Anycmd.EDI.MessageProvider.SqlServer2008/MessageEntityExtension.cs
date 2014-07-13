
namespace Anycmd.EDI.MessageProvider.SqlServer2008
{
    using Anycmd.Host.EDI.Handlers;
    using Anycmd.Host.EDI.Messages;
    using System;
    using System.Data;

    /// <summary>
    /// 
    /// </summary>
    internal static class MessageEntityExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandEntity"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        internal static DataRow ToDataRow<T>(this T commandEntity, DataTable dt) where T : MessageEntity
        {
            return SetCommandData(commandEntity, dt.NewRow());
        }

        #region private Methods
        private static DataRow SetCommandData(MessageEntity commandEntity, DataRow row)
        {
            row["Id"] = commandEntity.Id;
            row["Verb"] = commandEntity.Verb;
            row["InfoFormat"] = commandEntity.DataTuple.InfoFormat;
            row["InfoValue"] = commandEntity.DataTuple.ValueItems.InfoString;
            row["Ontology"] = commandEntity.Ontology;
            row["InfoID"] = commandEntity.DataTuple.IDItems.InfoString;
            row["LocalEntityID"] = commandEntity.LocalEntityID;
            row["OrganizationCode"] = commandEntity.OrganizationCode;
            row["ClientID"] = commandEntity.ClientID;
            row["TimeStamp"] = commandEntity.TimeStamp;
            row["ReceivedOn"] = commandEntity.ReceivedOn;
            row["CreateOn"] = commandEntity.CreateOn;
            row["ClientType"] = commandEntity.ClientType;
            row["MessageType"] = commandEntity.MessageType;
            row["MessageID"] = commandEntity.MessageID;
            row["Status"] = commandEntity.Status;
            row["ReasonPhrase"] = GetNullableValue(commandEntity.ReasonPhrase);
            row["Description"] = GetNullableValue(commandEntity.Description);
            row["EventSubjectCode"] = GetNullableValue(commandEntity.EventSubjectCode);
            row["EventSourceType"] = GetNullableValue(commandEntity.EventSourceType);
            row["UserName"] = GetNullableValue(commandEntity.UserName);
            row["QueryList"] = GetNullableValue(commandEntity.DataTuple.QueryListString);
            row["Version"] = commandEntity.Version;
            row["IsDumb"] = commandEntity.IsDumb;

            return row;
        }

        private static object GetNullableValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return DBNull.Value;
            }
            return value;
        }

        private static object GetNullableValue(DateTime? value)
        {
            if (value.HasValue)
            {
                return value.Value;
            }
            return DBNull.Value;
        }
        #endregion
    }
}

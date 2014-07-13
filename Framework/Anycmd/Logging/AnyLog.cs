
namespace Anycmd.Logging
{
    using Extensions;
    using System;
    using System.Data;
    using Util;

    /// <summary>
    /// 任何日志<see cref="IAnyLog"/>
    /// </summary>
    public class AnyLog : IAnyLog
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public AnyLog(Guid id)
        {
            this.Id = id;
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            AppDomain domain = AppDomain.CurrentDomain;
            this.Machine = Environment.MachineName;
            this.Process = process.ProcessName;
            this.BaseDirectory = domain.BaseDirectory;
            this.DynamicDirectory = domain.DynamicDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static AnyLog Create(IDataRecord reader)
        {
            return new AnyLog(reader.GetGuid(reader.GetOrdinal("Id")))
            {
                Machine = reader.GetNullableString(reader.GetOrdinal("Machine")),
                Process = reader.GetNullableString(reader.GetOrdinal("Process")),
                BaseDirectory = reader.GetNullableString(reader.GetOrdinal("BaseDirectory")),
                DynamicDirectory = reader.GetNullableString(reader.GetOrdinal("DynamicDirectory")),
                Req_Ontology = reader.GetNullableString(reader.GetOrdinal("Req_Ontology")),
                Req_Verb = reader.GetNullableString(reader.GetOrdinal("Req_Verb")),
                Req_ClientID = reader.GetNullableString(reader.GetOrdinal("Req_ClientID")),
                Req_ClientType = reader.GetNullableString(reader.GetOrdinal("Req_ClientType")),
                CreateOn = reader.GetNullableDateTime(reader.GetOrdinal("CreateOn"), SystemTime.MinDate).Value,
                Req_Description = reader.GetNullableString(reader.GetOrdinal("Req_Description")),
                Req_EventSourceType = reader.GetNullableString(reader.GetOrdinal("Req_EventSourceType")),
                Req_EventSubjectCode = reader.GetNullableString(reader.GetOrdinal("Req_EventSubjectCode")),
                InfoFormat = reader.GetNullableString(reader.GetOrdinal("InfoFormat")),
                Req_InfoID = reader.GetNullableString(reader.GetOrdinal("Req_InfoID")),
                Req_InfoValue = reader.GetNullableString(reader.GetOrdinal("Req_InfoValue")),
                Req_UserName = reader.GetNullableString(reader.GetOrdinal("Req_UserName")),
                Req_IsDumb = reader.GetBoolean(reader.GetOrdinal("Req_IsDumb")),
                LocalEntityID = reader.GetNullableString(reader.GetOrdinal("LocalEntityID")),
                OrganizationCode = reader.GetNullableString(reader.GetOrdinal("OrganizationCode")),
                Req_ReasonPhrase = reader.GetNullableString(reader.GetOrdinal("Req_ReasonPhrase")),
                ReceivedOn = reader.GetNullableDateTime(reader.GetOrdinal("ReceivedOn"), SystemTime.MinDate).Value,
                Req_MessageID = reader.GetNullableString(reader.GetOrdinal("Req_MessageID")),
                Req_MessageType = reader.GetNullableString(reader.GetOrdinal("Req_MessageType")),
                Req_QueryList = reader.GetNullableString(reader.GetOrdinal("Req_QueryList")),
                Req_Status = reader.GetNullableInt32(reader.GetOrdinal("Req_Status"), 0).Value,
                Req_TimeStamp = reader.GetNullableDateTime(reader.GetOrdinal("Req_TimeStamp"), SystemTime.MinDate).Value,
                Req_Version = reader.GetNullableString(reader.GetOrdinal("Req_Version")),
                Res_InfoValue = reader.GetNullableString(reader.GetOrdinal("Res_InfoValue")),
                Res_Description = reader.GetNullableString(reader.GetOrdinal("Res_Description")),
                Res_ReasonPhrase = reader.GetNullableString(reader.GetOrdinal("Res_ReasonPhrase")),
                Res_StateCode = reader.GetNullableInt32(reader.GetOrdinal("Res_StateCode"), 0).Value
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataRow ToDataRow(DataTable dt)
        {
            var row = dt.NewRow();
            row["Id"] = this.Id == Guid.Empty ? Guid.NewGuid() : this.Id;
            row["Machine"] = this.Machine;
            row["Process"] = this.Process;
            row["BaseDirectory"] = this.BaseDirectory;
            row["DynamicDirectory"] = this.DynamicDirectory;
            row["ReceivedOn"] = this.ReceivedOn;
            row["CreateOn"] = this.CreateOn;
            row["LocalEntityID"] = this.LocalEntityID;
            row["OrganizationCode"] = this.OrganizationCode;
            row["Req_Verb"] = this.Req_Verb;
            row["InfoFormat"] = this.InfoFormat;
            row["Req_InfoValue"] = this.Req_InfoValue;
            row["Req_Ontology"] = this.Req_Ontology;
            row["Req_InfoID"] = this.Req_InfoID;
            row["Req_ClientID"] = this.Req_ClientID;
            row["Req_TimeStamp"] = this.Req_TimeStamp;
            row["Req_ClientType"] = this.Req_ClientType;
            row["Req_MessageType"] = this.Req_MessageType;
            row["Req_MessageID"] = this.Req_MessageID;
            row["Req_Status"] = this.Req_Status;
            row["Req_ReasonPhrase"] = this.Req_ReasonPhrase;
            row["Req_Description"] = this.Req_Description;
            row["Req_EventSubjectCode"] = this.Req_EventSubjectCode;
            row["Req_EventSourceType"] = this.Req_EventSourceType;
            row["Req_UserName"] = this.Req_UserName;
            row["Req_QueryList"] = this.Req_QueryList;
            row["Req_Version"] = this.Req_Version;
            row["Req_IsDumb"] = this.Req_IsDumb;
            row["Res_StateCode"] = this.Res_StateCode;
            row["Res_ReasonPhrase"] = this.Res_ReasonPhrase;
            row["Res_Description"] = this.Res_Description;
            row["Res_InfoValue"] = this.Res_InfoValue;
            // 检测每一列对应的值是否超出了数据库定义的长度
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var dbCol = dt.Columns[i];
                if (dbCol.MaxLength != -1)
                {
                    if (row[i].ToString().Length > dbCol.MaxLength)
                    {
                        row[i] = row[i].ToString().Substring(0, dbCol.MaxLength);
                    }
                }
            }

            return row;
        }

        public Guid Id { get; set; }

        public string Machine { get; private set; }

        public string Process { get; private set; }

        public string BaseDirectory { get; private set; }

        public string DynamicDirectory { get; private set; }

        public string LocalEntityID { get; set; }

        public string OrganizationCode { get; set; }

        public DateTime ReceivedOn { get; set; }

        public DateTime CreateOn { get; set; }

        public string InfoFormat { get; set; }

        public string Req_Version { get; set; }

        public bool Req_IsDumb { get; set; }

        public string Req_MessageType { get; set; }

        public string Req_MessageID { get; set; }

        public string Req_ClientType { get; set; }

        public string Req_ClientID { get; set; }

        public int Req_Status { get; set; }

        public string Req_ReasonPhrase { get; set; }

        public string Req_Description { get; set; }

        public string Req_EventSubjectCode { get; set; }

        public string Req_EventSourceType { get; set; }

        public string Req_UserName { get; set; }

        public string Req_Verb { get; set; }

        public string Req_Ontology { get; set; }

        public DateTime Req_TimeStamp { get; set; }

        public string Req_QueryList { get; set; }

        public string Req_InfoID { get; set; }

        public string Req_InfoValue { get; set; }

        public int Res_StateCode { get; set; }

        public string Res_ReasonPhrase { get; set; }

        public string Res_Description { get; set; }

        public string Res_InfoValue { get; set; }
    }
}

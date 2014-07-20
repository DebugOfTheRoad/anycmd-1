
namespace Anycmd.EDI.ViewModels.ElementViewModels
{
    using Exceptions;
    using Host.AC.Infra;
    using Host.EDI;
    using Model;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public partial class ElementInfo : Dictionary<string, object>
    {
        private readonly IAppHost host;

        private ElementInfo(IAppHost host)
        {
            this.host = host;
        }

        public static ElementInfo Create(DicReader dic)
        {
            if (dic == null)
            {
                return null;
            }
            var data = new ElementInfo(dic.Host);
            foreach (var item in dic)
            {
                data.Add(item.Key, item.Value);
            }
            data.Id = (Guid)dic["Id"];
            OntologyDescriptor ontology;
            if (!dic.Host.Ontologies.TryGetOntology((Guid)data["OntologyID"], out ontology))
            {
                throw new CoreException("意外的本体标识" + data["OntologyID"]);
            }
            if (!data.ContainsKey("OntologyCode"))
            {
                data.Add("OntologyCode", ontology.Ontology.Code);
            }
            if (!data.ContainsKey("OntologyName"))
            {
                data.Add("OntologyName", ontology.Ontology.Name);
            }
            if (data["MaxLength"] == DBNull.Value)
            {
                data.MaxLength = null;
            }
            else
            {
                data.MaxLength = (int?)data["MaxLength"];
            }
            if (!data.ContainsKey("DeletionStateName"))
            {
                data.Add("DeletionStateName", dic.Host.Translate("EDI", "Element", "DeletionStateName", data["DeletionStateCode"].ToString()));
            }
            if (!data.ContainsKey("IsEnabledName"))
            {
                data.Add("IsEnabledName", dic.Host.Translate("EDI", "Element", "IsEnabledName", data["IsEnabled"].ToString()));
            }
            if (data["InfoDicID"] == DBNull.Value)
            {
                data.InfoDicID = null;
            }
            else
            {
                data.InfoDicID = (Guid?)data["InfoDicID"];
            }
            if (data.InfoDicID.HasValue && !data.ContainsKey("InfoDicName"))
            {
                InfoDicState infoDic;
                if (!dic.Host.InfoDics.TryGetInfoDic(data.InfoDicID.Value, out infoDic))
                {
                    throw new CoreException("意外的信息字典标识" + data.InfoDicID.Value);
                }
                data.Add("InfoDicName", infoDic.Name);
            }
            if (!data.ContainsKey("IsConfigValid"))
            {
                data.Add("IsConfigValid", data.IsConfigValid);
            }
            if (!data.ContainsKey("DbIsNullable"))
            {
                data.Add("DbIsNullable", data.DbIsNullable);
            }
            if (!data.ContainsKey("DbTypeName"))
            {
                data.Add("DbTypeName", data.DbTypeName);
            }
            if (!data.ContainsKey("DbMaxLength"))
            {
                data.Add("DbMaxLength", data.DbMaxLength);
            }

            return data;
        }
        private Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private int? MaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private Guid? InfoDicID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private bool IsConfigValid
        {
            get
            {
                bool isValid = true;
                if (DataSchema == null)
                {
                    isValid = false;
                }
                else if (DataSchema.MaxLength.HasValue && DataSchema.MaxLength > 0 && this.MaxLength > DataSchema.MaxLength)
                {
                    isValid = false;
                }

                return isValid;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool DbIsNullable
        {
            get
            {
                if (DataSchema == null)
                {
                    return false;
                }
                return DataSchema.IsNullable;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private string DbTypeName
        {
            get
            {
                if (DataSchema == null)
                {
                    return string.Empty;
                }
                return DataSchema.TypeName;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private int? DbMaxLength
        {
            get
            {
                if (DataSchema == null)
                {
                    return null;
                }
                return DataSchema.MaxLength;
            }
        }

        private ElementDataSchema _dataSchema;
        private ElementDataSchema DataSchema
        {
            get
            {
                if (_dataSchema == null)
                {
                    _dataSchema = host.Ontologies.GetElement(this.Id).DataSchema;
                }

                return _dataSchema;
            }
        }
    }
}

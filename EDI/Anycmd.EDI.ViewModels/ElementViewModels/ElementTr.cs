
namespace Anycmd.EDI.ViewModels.ElementViewModels
{
    using Exceptions;
    using Host.AC.Infra;
    using Host.EDI;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public partial class ElementTr
    {
        private readonly IAppHost host;

        public ElementTr(IAppHost host)
        {
            this.host = host;
        }

        public static ElementTr Create(ElementState element)
        {
            return new ElementTr(element.Host)
            {
                Nullable = element.Nullable,
                Code = element.Code,
                CreateOn = element.CreateOn,
                OType = element.OType,
                FieldCode = element.FieldCode,
                GroupID = element.GroupID ?? Guid.Empty,
                Icon = element.Icon,
                OntologyID = element.OntologyID,
                Id = element.Id,
                InfoDicID = element.InfoDicID,
                IsDetailsShow = element.IsDetailsShow,
                IsEnabled = element.IsEnabled,
                IsExport = element.IsExport,
                IsGridColumn = element.IsGridColumn,
                IsImport = element.IsImport,
                IsInfoIDItem = element.IsInfoIDItem,
                IsInput = element.IsInput,
                MaxLength = element.MaxLength,
                Name = element.Name,
                Ref = element.Ref,
                Regex = element.Regex,
                SortCode = element.SortCode
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FieldCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid OntologyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OntologyCode
        {
            get
            {
                return this.Ontology.Ontology.Code;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string OntologyName
        {
            get
            {
                return this.Ontology.Ontology.Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int? MaxLength { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool Nullable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string OType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid? InfoDicID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string InfoDicName
        {
            get
            {
                if (!this.InfoDicID.HasValue)
                {
                    return string.Empty;
                }
                return this.InfoDic.Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string InfoDicCode
        {
            get
            {
                if (!this.InfoDicID.HasValue)
                {
                    return string.Empty;
                }
                return this.InfoDic.Code;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int IsEnabled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Ref { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsInfoIDItem { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateOn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid GroupID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDetailsShow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsExport { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsImport { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsInput { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsGridColumn { get; set; }

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

        /// <summary>
        /// 
        /// </summary>
        public bool IsConfigValid
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
        public bool DbIsNullable
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
        public string DbTypeName
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
        public int? DbMaxLength
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

        private OntologyDescriptor _ontology;
        private OntologyDescriptor Ontology
        {
            get
            {
                if (_ontology == null)
                {
                    if (!host.Ontologies.TryGetOntology(this.OntologyID, out _ontology))
                    {
                        throw new CoreException("意外的本体标识" + this.OntologyID);
                    }
                }
                return _ontology;
            }
        }

        private InfoDicState _infoDic;
        private InfoDicState InfoDic
        {
            get
            {
                if (!this.InfoDicID.HasValue)
                {
                    return null;
                }
                if (_infoDic == null)
                {
                    if (!host.InfoDics.TryGetInfoDic(this.InfoDicID.Value, out _infoDic))
                    {
                        throw new CoreException("意外的信息字典标识" + this.InfoDicID);
                    }
                }
                return _infoDic;
            }
        }
    }
}

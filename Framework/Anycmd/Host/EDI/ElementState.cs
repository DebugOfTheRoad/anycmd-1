
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using Exceptions;
    using Hecp;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class ElementState : IElement
    {
        private Dictionary<Verb, ElementActionState> _elementActionDic;
        private List<ElementInfoRuleState> _elementInfoRuleList;
        private List<InfoRuleState> _infoRules;
        private Guid _ontologyID;
        private Guid? _infoDicID;

        private ElementState() { }

        public static ElementState Create(IAppHost host, ElementBase element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            var data = new ElementState
            {
                Host = host,
                Actions = element.Actions,
                AllowFilter = element.AllowFilter,
                AllowSort = element.AllowSort,
                Code = element.Code,
                CreateBy = element.CreateBy,
                CreateOn = element.CreateOn,
                CreateUserID = element.CreateUserID,
                DeletionStateCode = element.DeletionStateCode,
                Description = element.Description,
                FieldCode = element.FieldCode,
                ForeignElementID = element.ForeignElementID,
                GroupID = element.GroupID,
                Icon = element.Icon,
                Id = element.Id,
                InfoChecks = element.InfoChecks,
                InfoDicID = element.InfoDicID,
                InfoRules = element.InfoRules,
                InputHeight = element.InputHeight,
                InputType = element.InputType,
                InputWidth = element.InputWidth,
                IsDetailsShow = element.IsDetailsShow,
                IsEnabled = element.IsEnabled,
                IsExport = element.IsExport,
                IsGridColumn = element.IsGridColumn,
                IsImport = element.IsImport,
                IsInfoIDItem = element.IsInfoIDItem,
                IsInput = element.IsInput,
                IsTotalLine = element.IsTotalLine,
                MaxLength = element.MaxLength,
                ModifiedBy = element.ModifiedBy,
                ModifiedOn = element.ModifiedOn,
                ModifiedUserID = element.ModifiedUserID,
                Name = element.Name,
                Nullable = element.Nullable,
                OntologyID = element.OntologyID,
                OType = element.OType,
                Ref = element.Ref,
                Regex = element.Regex,
                SortCode = element.SortCode,
                Tooltip = element.Tooltip,
                Triggers = element.Triggers,
                UniqueElementIDs = element.UniqueElementIDs,
                Width = element.Width
            };
            var elementActionDic = new Dictionary<Verb, ElementActionState>();
            data._elementActionDic = elementActionDic;
            if (data.Actions != null)
            {
                var elementActions = host.DeserializeFromString<ElementAction[]>(data.Actions);
                if (elementActions != null)
                {
                    foreach (var elementAction in elementActions)
                    {
                        OntologyDescriptor ontology;
                        if (!host.Ontologies.TryGetOntology(data.OntologyID, out ontology))
                        {
                            throw new CoreException("意外的本体元素本体标识" + data.OntologyID);
                        }
                        var actionDic = host.Ontologies.GetActons(ontology);
                        var verb = actionDic.Where(a => a.Value.Id == elementAction.ActionID).Select(a => a.Key).FirstOrDefault();
                        if (verb != null)
                        {
                            elementActionDic.Add(verb, ElementActionState.Create(elementAction));
                        }
                    }
                    if (elementActions.Count() != elementActionDic.Count)
                    {
                        // TODO:发现无效数据，重新序列化并持久化
                    }
                }
            }
            var elementInfoRuleList = new List<ElementInfoRuleState>();
            var infoRules = new List<InfoRuleState>();
            data._elementInfoRuleList = elementInfoRuleList;
            data._infoRules = infoRules;
            if (data.InfoRules != null)
            {
                var elementInfoRules = host.DeserializeFromString<ElementInfoRule[]>(data.InfoRules);
                if (elementInfoRules != null)
                {
                    foreach (var elementInfoRule in elementInfoRules)
                    {
                        InfoRuleState infoRule;
                        if (host.InfoRules.TryGetInfoRule(elementInfoRule.InfoRuleID, out infoRule))
                        {
                            elementInfoRuleList.Add(ElementInfoRuleState.Create(host, elementInfoRule));
                            infoRules.Add(infoRule);
                        }
                    }
                    if (elementInfoRules.Count() != elementInfoRuleList.Count)
                    {
                        // TODO:发现无效数据，重新序列化并持久化
                    }
                }
            }
            return data;
        }

        #region IElement 成员
        public Guid Id { get; private set; }

        public IAppHost Host { get; private set; }

        public Guid OntologyID
        {
            get { return _ontologyID; }
            private set
            {
                OntologyDescriptor ontology;
                if (!Host.Ontologies.TryGetOntology(value, out ontology))
                {
                    throw new ValidationException("意外的本体标识" + value);
                }
                _ontologyID = value;
            }
        }

        /// <summary>
        /// 引用本体元素
        /// </summary>
        public Guid? ForeignElementID { get; private set; }

        public string Actions { get; private set; }

        public IReadOnlyDictionary<Verb, ElementActionState> ElementActions
        {
            get { return _elementActionDic; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string UniqueElementIDs { get; private set; }

        public string InfoChecks { get; private set; }

        public string InfoRules { get; private set; }

        public IReadOnlyCollection<IElementInfoRule> ElementInfoRules
        {
            get { return _elementInfoRuleList; }
        }

        public IReadOnlyCollection<InfoRuleState> GetInfoRules()
        {
            return _infoRules;
        }

        public string Triggers { get; private set; }

        public string Code { get; private set; }

        public string FieldCode { get; private set; }

        public string Name { get; private set; }

        public string Ref { get; private set; }

        public int? MaxLength { get; private set; }

        public string OType { get; private set; }

        public bool Nullable { get; private set; }

        public Guid? InfoDicID
        {
            get { return _infoDicID; }
            private set
            {
                if (value.HasValue)
                {
                    InfoDicState infoDic;
                    if (!Host.InfoDics.TryGetInfoDic(value.Value, out infoDic))
                    {
                        throw new ValidationException("意外的信息字典标识" + value);
                    }
                }
                _infoDicID = value;
            }
        }

        public string Regex { get; private set; }

        public bool IsInfoIDItem { get; private set; }

        public int SortCode { get; private set; }

        public string Description { get; private set; }

        public int DeletionStateCode { get; private set; }

        public int IsEnabled { get; private set; }

        public DateTime? CreateOn { get; private set; }

        public Guid? CreateUserID { get; private set; }

        public string CreateBy { get; private set; }

        public DateTime? ModifiedOn { get; private set; }

        public Guid? ModifiedUserID { get; private set; }

        public string ModifiedBy { get; private set; }

        public Guid? GroupID { get; private set; }

        public string Tooltip { get; set; }

        public string Icon { get; private set; }

        public bool IsDetailsShow { get; private set; }

        public bool IsExport { get; private set; }

        public bool IsImport { get; private set; }

        public bool IsInput { get; private set; }

        public string InputType { get; private set; }

        public bool IsTotalLine { get; private set; }

        public int? InputWidth { get; private set; }

        public int? InputHeight { get; private set; }

        public bool IsGridColumn { get; private set; }

        public int Width { get; private set; }

        public bool AllowSort { get; private set; }

        public bool AllowFilter { get; private set; }
        #endregion

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var value = obj as ElementState;
            if (value == null)
            {
                return false;
            }
            var left = this;
            var right = value;

            return left.Id == right.Id &&
                left.OntologyID == right.OntologyID &&
                left.ForeignElementID == right.ForeignElementID &&
                left.Actions == right.Actions &&
                left.UniqueElementIDs == right.UniqueElementIDs &&
                left.InfoChecks == right.InfoChecks &&
                left.InfoRules == right.InfoRules &&
                left.Triggers == right.Triggers &&
                left.Code == right.Code &&
                left.FieldCode == right.FieldCode &&
                left.Name == right.Name &&
                left.Ref == right.Ref &&
                left.MaxLength == right.MaxLength &&
                left.OType == right.OType &&
                left.Nullable == right.Nullable &&
                left.InfoDicID == right.InfoDicID &&
                left.Regex == right.Regex &&
                left.IsInfoIDItem == right.IsInfoIDItem &&
                left.SortCode == right.SortCode &&
                left.IsEnabled == right.IsEnabled &&
                left.GroupID == right.GroupID &&
                left.Tooltip == right.Tooltip &&
                left.Icon == right.Icon &&
                left.IsDetailsShow == right.IsDetailsShow &&
                left.IsExport == right.IsExport &&
                left.IsImport == right.IsImport &&
                left.IsInput == right.IsInput &&
                left.InputType == right.InputType &&
                left.IsTotalLine == right.IsTotalLine &&
                left.InputWidth == right.InputWidth &&
                left.InputHeight == right.InputHeight &&
                left.IsGridColumn == right.IsGridColumn &&
                left.Width == right.Width &&
                left.AllowSort == right.AllowSort &&
                left.AllowFilter == right.AllowFilter;
        }

        public static bool operator ==(ElementState a, ElementState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(ElementState a, ElementState b)
        {
            return !(a == b);
        }
    }
}

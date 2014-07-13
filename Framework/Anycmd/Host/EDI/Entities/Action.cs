
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 动作码
    /// </summary>
    public class Action : ActionBase, IAggregateRoot
    {
        public Action() { }

        public static Action Create(IActionCreateInput input)
        {
            return new Action
            {
                IsAllowed = input.IsAllowed,
                IsAudit = input.IsAudit,
                Id = input.Id.Value,
                Description = input.Description,
                IsPersist = input.IsPersist,
                Name = input.Name,
                OntologyID = input.OntologyID,
                SortCode = input.SortCode,
                Verb = input.Verb
            };
        }

        public void Update(IActionUpdateInput input)
        {
            this.Description = input.Description;
            this.IsAllowed = input.IsAllowed;
            this.IsPersist = input.IsPersist;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
            this.Verb = input.Verb;
        }
    }
}

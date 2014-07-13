
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 
    /// </summary>
    public class Topic : TopicBase, IAggregateRoot
    {
        public Topic() { }

        public static Topic Create(ITopicCreateInput input)
        {
            return new Topic
            {
                Code = input.Code,
                Id = input.Id.Value,
                Description = input.Description,
                IsAllowed = input.IsAllowed,
                Name = input.Name,
                OntologyID = input.OntologyID
            };
        }

        public void Update(ITopicUpdateInput input)
        {
            this.Code = input.Code;
            this.Name = input.Name;
            this.Description = input.Description;
        }
    }
}

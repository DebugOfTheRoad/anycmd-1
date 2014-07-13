
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 信息组。本体元素的分组
    /// </summary>
    public class InfoGroup : InfoGroupBase, IAggregateRoot
    {
        public InfoGroup() { }

        public static InfoGroup Create(IInfoGroupCreateInput input)
        {
            return new InfoGroup
            {
                Code = input.Code,
                Id = input.Id.Value,
                Description = input.Description,
                Name = input.Name,
                SortCode = input.SortCode,
                OntologyID = input.OntologyID
            };
        }

        public void Update(IInfoGroupUpdateInput input)
        {
            this.Code = input.Code;
            this.Description = input.Description;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
        }
    }
}

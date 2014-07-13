
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 系统字典项
    /// </summary>
    public class DicItem : DicItemBase, IAggregateRoot
    {
        public DicItem()
        {
        }

        public static DicItem Create(IDicItemCreateInput input)
        {
            return new DicItem
            {
                Id = input.Id.Value,
                Code = input.Code,
                Name = input.Name,
                DicID = input.DicID,
                Description = input.Description,
                IsEnabled = input.IsEnabled,
                SortCode = input.SortCode
            };
        }

        public void Update(IDicItemUpdateInput input)
        {
            this.Code = input.Code;
            this.Description = input.Description;
            this.IsEnabled = input.IsEnabled;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
        }
    }
}

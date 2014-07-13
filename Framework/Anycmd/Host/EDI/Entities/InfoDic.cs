
namespace Anycmd.Host.EDI.Entities
{
    using Anycmd.EDI;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 信息字典
    /// <remarks>
    /// 有些本体元素在实体上的取值不是任意的。当本体是“人”时，人有“民族”这个本体元素，本体元素“民族”的“数据类型”是字典型的。
    /// “教师”本体是“人”本体的一个子类，张老师是一个“实体人”，张老师的“民族属性”取值就不是任意的而是由教育部的“民族”字典限定的。
    /// </remarks>
    /// </summary>
    public class InfoDic : InfoDicBase, IAggregateRoot
    {
        public InfoDic() { }

        public static InfoDic Create(IInfoDicCreateInput input)
        {
            return new InfoDic
            {
                Code = input.Code,
                Id = input.Id.Value,
                IsEnabled = input.IsEnabled,
                Name = input.Name,
                Description = input.Description,
                SortCode = input.SortCode
            };
        }

        public void Update(IInfoDicUpdateInput input)
        {
            this.Code = input.Code;
            this.Description = input.Description;
            this.IsEnabled = input.IsEnabled;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
        }
    }
}

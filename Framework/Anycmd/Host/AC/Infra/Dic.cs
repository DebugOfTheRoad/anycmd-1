
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 系统字典
    /// </summary>
    public class Dic : DicBase, IAggregateRoot
    {
        public Dic()
        {
        }

        public static Dic Create(IDicCreateInput input)
        {
            return new Dic
                {
                    Id = input.Id.Value,
                    Code = input.Code,
                    Name = input.Name,
                    Description = input.Description,
                    SortCode = input.SortCode,
                    IsEnabled = input.IsEnabled
                };
        }

        public void Update(IDicUpdateInput input)
        {
            this.Code = input.Code;
            this.Description = input.Description;
            this.IsEnabled = input.IsEnabled;
            this.SortCode = input.SortCode;
            this.Name = input.Name;
        }
    }
}

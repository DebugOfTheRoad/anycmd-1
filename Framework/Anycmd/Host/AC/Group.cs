
namespace Anycmd.Host.AC
{
    using Anycmd.AC;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 工作组模型
    /// </summary>
    public class Group : GroupBase, IAggregateRoot, IGroup
    {
        public Group()
        {
            IsEnabled = 1;
        }

        public static Group Create(IGroupCreateInput input)
        {
            return new Group
            {
                Id = input.Id.Value,
                OrganizationCode = input.OrganizationCode,
                CategoryCode = input.CategoryCode,
                Description = input.Description,
                IsEnabled = input.IsEnabled,
                Name = input.Name,
                ShortName = input.ShortName,
                SortCode = input.SortCode,
                TypeCode = input.TypeCode,
                PrivilegeState = null
            };
        }

        public void Update(IGroupUpdateInput input)
        {
            this.CategoryCode = input.CategoryCode;
            this.OrganizationCode = input.OrganizationCode;
            this.Description = input.Description;
            this.IsEnabled = input.IsEnabled;
            this.Name = input.Name;
            this.ShortName = input.ShortName;
            this.SortCode = input.SortCode;
            this.TypeCode = input.TypeCode;
        }
    }
}


namespace Anycmd.Host.AC
{
    using Anycmd.AC;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 角色
    /// </summary>
    public class Role : RoleBase, IAggregateRoot
    {
        public Role()
        {
            AllowDelete = 1;
            AllowEdit = 1;
            IsEnabled = 1;
        }

        public static Role Create(IRoleCreateInput input)
        {
            return new Role
            {
                AllowDelete = 1,
                AllowEdit = 1,
                CategoryCode = input.CategoryCode,
                Description = input.Description,
                Icon = input.Icon,
                Id = input.Id.Value,
                IsEnabled = input.IsEnabled,
                Name = input.Name,
                PrivilegeState = null,
                SortCode = input.SortCode
            };
        }

        public void Update(IRoleUpdateInput input)
        {
            this.CategoryCode = input.CategoryCode;
            this.Description = input.Description;
            this.Icon = input.Icon;
            this.IsEnabled = input.IsEnabled;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
        }
    }
}

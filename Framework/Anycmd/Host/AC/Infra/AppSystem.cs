
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 应用系统模型
    /// </summary>
    public class AppSystem : AppSystemBase, IAggregateRoot, IAppSystem
    {
        public AppSystem()
        {
            AllowDelete = 1;
            AllowEdit = 1;
        }

        public static AppSystem Create(IAppSystemCreateInput input)
        {
            return new AppSystem
            {
                Code = input.Code,
                Id = input.Id.Value,
                Name = input.Name,
                Description = input.Description,
                Icon = input.Icon,
                PrincipalID = input.PrincipalID,
                SSOAuthAddress = input.SSOAuthAddress,
                IsEnabled = input.IsEnabled,
                SortCode = input.SortCode,
                ImageUrl = input.ImageUrl
            };
        }

        public void Update(IAppSystemUpdateInput input)
        {
            this.Code = input.Code;
            this.Description = input.Description;
            this.Icon = input.Icon;
            this.ImageUrl = input.ImageUrl;
            this.IsEnabled = input.IsEnabled;
            this.Name = input.Name;
            this.PrincipalID = input.PrincipalID;
            this.SortCode = input.SortCode;
            this.SSOAuthAddress = input.SSOAuthAddress;
        }
    }
}

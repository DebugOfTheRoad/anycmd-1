
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 资源。设计用于支持资源级配置。操作是附属于资源上的。
    /// </summary>
    public class ResourceType : ResourceTypeBase, IAggregateRoot
    {
        #region Ctor
        public ResourceType()
        {
            AllowDelete = 1;
            AllowEdit = 1;
        }
        #endregion

        public static ResourceType Create(IResourceCreateInput input)
        {
            return new ResourceType
            {
                Id = input.Id.Value,
                Code = input.Code,
                AppSystemID = input.AppSystemID,
                AllowDelete = 1,
                AllowEdit = 1,
                Icon = input.Icon,
                Description = input.Description,
                Name = input.Name,
                SortCode = input.SortCode
            };
        }

        public void Update(IResourceUpdateInput input)
        {
            this.Code = input.Code;
            this.Description = input.Description;
            this.Icon = input.Icon;
            this.Name = input.Name;
            this.SortCode = input.SortCode;
        }
    }
}

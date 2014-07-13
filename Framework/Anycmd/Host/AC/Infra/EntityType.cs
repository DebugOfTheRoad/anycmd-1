
namespace Anycmd.Host.AC.Infra
{
    using Anycmd.AC.Infra;
    using Model;
    using ValueObjects;

    /// <summary>
    /// 模型。设计用于支持模型级配置和界面自动生成等。
    /// <remarks>该模型是程序开发模型，被程序员使用，用户不关心本概念。</remarks>
    /// </summary>
    public class EntityType : EntityTypeBase, IAggregateRoot
    {
        public EntityType() { }

        public static EntityType Create(IEntityTypeCreateInput input)
        {
            return new EntityType
            {
                Id = input.Id.Value,
                Code = input.Code,
                Codespace = input.Codespace,
                DatabaseID = input.DatabaseID,
                Description = input.Description,
                DeveloperID = input.DeveloperID,
                IsOrganizational = input.IsOrganizational,
                Name = input.Name,
                EditHeight = input.EditHeight,
                EditWidth = input.EditWidth,
                SchemaName = input.SchemaName,
                TableName = input.TableName,
                SortCode = input.SortCode
            };
        }

        public void Update(IEntityTypeUpdateInput input)
        {
            this.Code = input.Code;
            this.Codespace = input.Codespace;
            this.IsOrganizational = input.IsOrganizational;
            this.DatabaseID = input.DatabaseID;
            this.Description = input.Description;
            this.DeveloperID = input.DeveloperID;
            this.EditWidth = input.EditWidth;
            this.EditHeight = input.EditHeight;
            this.Name = input.Name;
            this.SchemaName = input.SchemaName;
            this.SortCode = input.SortCode;
            this.TableName = input.TableName;
        }
    }
}

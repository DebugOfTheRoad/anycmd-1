
namespace Anycmd.AC.Infra.ViewModels.EntityTypeViewModels
{
    using Anycmd.Host;
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class EntityTypeTr
    {
        private readonly AppHost host;

        private EntityTypeTr(AppHost host)
        {
            this.host = host;
        }

        public static EntityTypeTr Create(EntityTypeState entityType)
        {
            if (entityType == null)
            {
                return null;
            }
            return new EntityTypeTr(entityType.AppHost)
            {
                Code = entityType.Code,
                Codespace = entityType.Codespace,
                IsOrganizational = entityType.IsOrganizational,
                CreateOn = entityType.CreateOn,
                DatabaseID = entityType.DatabaseID,
                DeveloperID = entityType.DeveloperID,
                Id = entityType.Id,
                Name = entityType.Name,
                SchemaName = entityType.SchemaName,
                SortCode = entityType.SortCode,
                TableName = entityType.TableName,
                ClrTypeFullName = entityType.Map.ClrType.FullName
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual Guid Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Codespace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsOrganizational { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DatabaseID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string SchemaName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual string TableName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual int SortCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual Guid DeveloperID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime? CreateOn { get; set; }

        public virtual string DeveloperCode
        {
            get
            {
                AccountState developer;
                if (!host.SysUsers.TryGetDevAccount(this.DeveloperID, out developer))
                {
                    return "无效值";
                }
                return developer.LoginName;
            }
        }

        public virtual string ClrTypeFullName { get; set; }
    }
}


namespace Anycmd.Host
{
    using Anycmd.AC.Infra;
    using Exceptions;
    using Host.AC.Infra;
    using System;
    using Util;

    public sealed class EntityTypeState : IEntityType
    {
        public static readonly EntityTypeState Empty = new EntityTypeState
        {
            Codespace = string.Empty,
            Code = string.Empty,
            IsOrganizational = false,
            CreateOn = SystemTime.MinDate,
            DatabaseID = Guid.Empty,
            DeveloperID = Guid.Empty,
            EditHeight = 0,
            EditWidth = 0,
            Id = Guid.Empty,
            Name = string.Empty,
            SchemaName = string.Empty,
            SortCode = 0,
            TableName = string.Empty
        };

        private EntityTypeMap _map;

        private EntityTypeState() { }

        public static EntityTypeState Create(AppHost host, EntityTypeBase entityType, EntityTypeMap map)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }
            if (!host.Rdbs.ContainsDb(entityType.DatabaseID))
            {
                throw new CoreException("意外的数据库" + entityType.DatabaseID);
            }

            return new EntityTypeState
            {
                AppHost = host,
                Map = map,
                Codespace = entityType.Codespace,
                Code = entityType.Code,
                IsOrganizational = entityType.IsOrganizational,
                CreateOn = entityType.CreateOn,
                DatabaseID = entityType.DatabaseID,
                DeveloperID = entityType.DeveloperID,
                EditHeight = entityType.EditHeight,
                EditWidth = entityType.EditWidth,
                Id = entityType.Id,
                Name = entityType.Name,
                SchemaName = entityType.SchemaName,
                SortCode = entityType.SortCode,
                TableName = entityType.TableName
            };
        }

        public AppHost AppHost { get; private set; }

        public EntityTypeMap Map
        {
            get
            {
                if (_map == null)
                {
                    return EntityTypeMap.Empty;
                }
                return _map;
            }
            private set { _map = value; }
        }

        public Guid Id { get; private set; }

        public string Codespace { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public bool IsOrganizational { get; private set; }

        public Guid DatabaseID { get; private set; }

        public Guid DeveloperID { get; private set; }

        public string SchemaName { get; private set; }

        public string TableName { get; private set; }

        public int SortCode { get; private set; }

        public int EditWidth { get; private set; }

        public int EditHeight { get; private set; }

        public DateTime? CreateOn { get; private set; }

        #region override Equals and GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (!(obj is EntityTypeState))
            {
                return false;
            }
            var left = this;
            var right = (EntityTypeState)obj;

            return left.Id == right.Id &&
                left.Codespace == right.Codespace &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.IsOrganizational == right.IsOrganizational &&
                left.DatabaseID == right.DatabaseID &&
                left.SchemaName == right.SchemaName &&
                left.TableName == right.TableName &&
                left.SortCode == right.SortCode &&
                left.DeveloperID == right.DeveloperID &&
                left.EditHeight == right.EditHeight &&
                left.EditWidth == right.EditWidth;
        }

        public static bool operator ==(EntityTypeState a, EntityTypeState b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(EntityTypeState a, EntityTypeState b)
        {
            return !(a == b);
        }
        #endregion
    }
}

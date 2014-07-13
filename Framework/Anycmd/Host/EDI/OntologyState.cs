
namespace Anycmd.Host.EDI
{
    using Anycmd.EDI;
    using System;

    public sealed class OntologyState : IOntology
    {
        private OntologyState() { }

        public static OntologyState Create(IOntology ontology)
        {
            if (ontology == null)
            {
                throw new ArgumentNullException("ontology");
            }
            return new OntologyState
            {
                CanAction = ontology.CanAction,
                CanCommand = ontology.CanCommand,
                CanEvent = ontology.CanEvent,
                Code = ontology.Code,
                CreateOn = ontology.CreateOn,
                DispatcherLoadCount = ontology.DispatcherLoadCount,
                DispatcherSleepTimeSpan = ontology.DispatcherSleepTimeSpan,
                EntityDatabaseID = ontology.EntityDatabaseID,
                EntityProviderID = ontology.EntityProviderID,
                EntitySchemaName = ontology.EntitySchemaName,
                EntityTableName = ontology.EntityTableName,
                ExecutorLoadCount = ontology.ExecutorLoadCount,
                ExecutorSleepTimeSpan = ontology.ExecutorSleepTimeSpan,
                Icon = ontology.Icon,
                Id = ontology.Id,
                IsEnabled = ontology.IsEnabled,
                IsLogicalDeletionEntity = ontology.IsLogicalDeletionEntity,
                IsOrganizationalEntity = ontology.IsOrganizationalEntity,
                IsSystem = ontology.IsSystem,
                MessageDatabaseID = ontology.MessageDatabaseID,
                MessageProviderID = ontology.MessageProviderID,
                MessageSchemaName = ontology.MessageSchemaName,
                Name = ontology.Name,
                ReceivedMessageBufferSize = ontology.ReceivedMessageBufferSize,
                ServiceIsAlive = ontology.ServiceIsAlive,
                SortCode = ontology.SortCode,
                Triggers = ontology.Triggers
            };
        }

        public Guid Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public string Triggers { get; private set; }

        public string Icon { get; private set; }

        public bool ServiceIsAlive { get; private set; }

        public Guid MessageProviderID { get; private set; }

        public Guid EntityProviderID { get; private set; }

        public Guid EntityDatabaseID { get; private set; }

        public bool IsSystem { get; private set; }

        public bool IsOrganizationalEntity { get; private set; }

        public bool IsLogicalDeletionEntity { get; private set; }

        public Guid MessageDatabaseID { get; private set; }

        public int ReceivedMessageBufferSize { get; private set; }

        public string EntitySchemaName { get; private set; }

        public string MessageSchemaName { get; private set; }

        public string EntityTableName { get; private set; }

        public int ExecutorLoadCount { get; private set; }

        public int ExecutorSleepTimeSpan { get; private set; }

        public int DispatcherLoadCount { get; private set; }

        public int DispatcherSleepTimeSpan { get; private set; }

        public int IsEnabled { get; private set; }

        public bool CanAction { get; private set; }

        public bool CanCommand { get; private set; }

        public bool CanEvent { get; private set; }

        public int SortCode { get; private set; }

        public DateTime? CreateOn { get; private set; }

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
            var value = obj as OntologyState;
            if (value == null)
            {
                return false;
            }
            var left = this;
            var right = value;

            return left.Id == right.Id &&
                left.Code == right.Code &&
                left.Name == right.Name &&
                left.Triggers == right.Triggers &&
                left.Icon == right.Icon &&
                left.ServiceIsAlive == right.ServiceIsAlive &&
                left.MessageProviderID == right.MessageProviderID &&
                left.EntityProviderID == right.EntityProviderID &&
                left.EntityDatabaseID == right.EntityDatabaseID &&
                left.IsSystem == right.IsSystem &&
                left.IsOrganizationalEntity == right.IsOrganizationalEntity &&
                left.IsLogicalDeletionEntity == right.IsLogicalDeletionEntity &&
                left.MessageDatabaseID == right.MessageDatabaseID &&
                left.ReceivedMessageBufferSize == right.ReceivedMessageBufferSize &&
                left.EntitySchemaName == right.EntitySchemaName &&
                left.MessageSchemaName == right.MessageSchemaName &&
                left.EntityTableName == right.EntityTableName &&
                left.ExecutorLoadCount == right.ExecutorLoadCount &&
                left.ExecutorSleepTimeSpan == right.ExecutorSleepTimeSpan &&
                left.DispatcherLoadCount == right.DispatcherLoadCount &&
                left.DispatcherSleepTimeSpan == right.DispatcherSleepTimeSpan &&
                left.IsEnabled == right.IsEnabled &&
                left.CanAction == right.CanAction &&
                left.CanCommand == right.CanCommand &&
                left.CanEvent == right.CanEvent &&
                left.SortCode == right.SortCode;
        }

        public static bool operator ==(OntologyState a, OntologyState b)
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

        public static bool operator !=(OntologyState a, OntologyState b)
        {
            return !(a == b);
        }
    }
}

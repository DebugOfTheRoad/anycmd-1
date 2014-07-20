
namespace Anycmd
{
    using Commands;
    using Container;
    using Events;
    using Events.Serialization;
    using Events.Storage;
    using Host;
    using Host.AC.Infra;
    using Model;
    using Snapshots;
    using Snapshots.Serialization;
    using System;
    using System.Collections.Generic;
    using Util;

    public static class AppHostExtension
    {
        private static readonly HashSet<EntityTypeMap> _entityTypeMaps = new HashSet<EntityTypeMap>();

        public static void Map(this IAppHost host, EntityTypeMap map)
        {
            _entityTypeMaps.Add(map);
        }

        public static IEnumerable<EntityTypeMap> GetEntityTypeMaps(this IAppHost host)
        {
            return _entityTypeMaps;
        }

        public static T DeserializeFromString<T>(this IAppHost host, string value)
        {
            // TODO:移除对ServiceStack.Text的依赖
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
        }

        public static string SerializeToString<T>(this IAppHost host, T value)
        {
            return ServiceStack.Text.JsonSerializer.SerializeToString<T>(value);
        }

        /// <summary>
        /// this.DirectEventBus.Publish(evnt);
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="evnt"></param>
        public static void PublishEvent<TEvent>(this IAppHost host, TEvent evnt) where TEvent : class, IEvent
        {
            host.EventBus.Publish(evnt);
        }

        /// <summary>
        /// this.DirectEventBus.Commit();
        /// </summary>
        public static void CommitEventBus(this IAppHost host)
        {
            host.EventBus.Commit();
        }

        /// <summary>
        /// this.DirectCommandBus.Publish(command);
        /// this.DirectCommandBus.Commit();
        /// </summary>
        /// <param name="command"></param>
        public static void Handle(this IAppHost host, ISysCommand command)
        {
            host.CommandBus.Publish(command);
            host.CommandBus.Commit();
        }

        /// <summary>
        /// Retrieves the service of type <c>T</c> from the provider.
        /// If the service cannot be found, this method returns <c>null</c>.
        /// </summary>
        public static T GetService<T>(this IAppHost host)
        {
            return (T)host.GetService(typeof(T));
        }

        /// <summary>
        /// Retrieves the service of type <c>T</c> from the provider.
        /// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
        /// </summary>
        public static T GetRequiredService<T>(this IAppHost host)
        {
            return (T)GetRequiredService(host, typeof(T));
        }

        /// <summary>
        /// Retrieves the service of type <paramref name="serviceType"/> from the provider.
        /// If the service cannot be found, a <see cref="ServiceNotFoundException"/> will be thrown.
        /// </summary>
        public static object GetRequiredService(this IAppHost host, Type serviceType)
        {
            object service = host.GetService(serviceType);
            if (service == null)
                throw new ServiceNotFoundException(serviceType);
            return service;
        }
        /// <summary>
        /// Creates and initializes the domain event data object from the given domain event.
        /// </summary>
        /// <param name="entity">The domain event instance from which the domain event data object
        /// is created and initialized.</param>
        /// <returns>The initialized data object instance.</returns>
        public static DomainEventDataObject FromDomainEvent(this IAppHost host, IDomainEvent entity)
        {
            IDomainEventSerializer serializer = host.GetRequiredService<IDomainEventSerializer>();
            DomainEventDataObject obj = new DomainEventDataObject();
            obj.Branch = entity.Branch;
            obj.Data = serializer.Serialize(entity);
            obj.Id = entity.Id;
            if (string.IsNullOrEmpty(entity.AssemblyQualifiedEventType))
                obj.AssemblyQualifiedEventType = entity.GetType().AssemblyQualifiedName;
            else
                obj.AssemblyQualifiedEventType = entity.AssemblyQualifiedEventType;
            obj.Timestamp = entity.Timestamp;
            obj.Version = entity.Version;
            obj.SourceID = entity.Source.Id;
            obj.AssemblyQualifiedSourceType = entity.Source.GetType().AssemblyQualifiedName;
            return obj;
        }
        /// <summary>
        /// Converts the domain event data object to its corresponding domain event entity instance.
        /// </summary>
        /// <returns>The domain event entity instance that is converted from the current domain event data object.</returns>
        public static IDomainEvent ToDomainEvent(this IAppHost host, DomainEventDataObject from)
        {
            if (string.IsNullOrEmpty(from.AssemblyQualifiedEventType))
                throw new ArgumentNullException("AssemblyQualifiedTypeName");
            if (from.Data == null || from.Data.Length == 0)
                throw new ArgumentNullException("Data");

            IDomainEventSerializer serializer = host.GetRequiredService<IDomainEventSerializer>();
            Type type = Type.GetType(from.AssemblyQualifiedEventType);
            IDomainEvent ret = (IDomainEvent)serializer.Deserialize(type, from.Data);
            ret.Id = from.Id;
            return ret;
        }

        /// <summary>
        /// Extracts the snapshot from the current snapshot data object.
        /// </summary>
        /// <returns>The snapshot instance.</returns>
        public static ISnapshot ExtractSnapshot(this IAppHost host, SnapshotDataObject dataObject)
        {
            try
            {
                ISnapshotSerializer serializer = host.GetRequiredService<ISnapshotSerializer>();
                Type snapshotType = Type.GetType(dataObject.SnapshotType);
                if (snapshotType == null)
                    return null;
                return (ISnapshot)serializer.Deserialize(snapshotType, dataObject.SnapshotData);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// Creates the snapshot data object from the aggregate root.
        /// </summary>
        /// <param name="aggregateRoot">The aggregate root for which the snapshot is being created.</param>
        /// <returns>The snapshot data object.</returns>
        public static SnapshotDataObject CreateFromAggregateRoot(this IAppHost host, ISourcedAggregateRoot aggregateRoot)
        {
            ISnapshotSerializer serializer = host.GetRequiredService<ISnapshotSerializer>();

            ISnapshot snapshot = aggregateRoot.CreateSnapshot();

            return new SnapshotDataObject
            {
                AggregateRootID = aggregateRoot.Id,
                AggregateRootType = aggregateRoot.GetType().AssemblyQualifiedName,
                Version = aggregateRoot.Version,
                Branch = Constants.ApplicationRuntime.DefaultBranch,
                SnapshotType = snapshot.GetType().AssemblyQualifiedName,
                Timestamp = snapshot.Timestamp,
                SnapshotData = serializer.Serialize(snapshot)
            };
        }
    }
}

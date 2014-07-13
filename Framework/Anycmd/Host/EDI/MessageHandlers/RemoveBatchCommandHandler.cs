
namespace Anycmd.Host.EDI.MessageHandlers
{
    using Anycmd.Repositories;
    using Commands;
    using Entities;
    using Messages;

    public class RemoveBatchCommandHandler : CommandHandler<RemoveBatchCommand>
    {
        private readonly AppHost host;

        public RemoveBatchCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(RemoveBatchCommand command)
        {
            var batchRepository = NodeHost.Instance.GetRequiredService<IRepository<Batch>>();
            var entity = batchRepository.GetByKey(command.EntityID);
            if (entity == null)
            {
                return;
            }
            batchRepository.Remove(entity);
            batchRepository.Context.Commit();

            host.PublishOperatedEvent(command.EntityID);
            host.PublishEvent(new BatchRemovedEvent(entity));
            host.CommitEventBus();
        }
    }
}

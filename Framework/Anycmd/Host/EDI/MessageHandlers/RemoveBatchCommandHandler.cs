
namespace Anycmd.Host.EDI.MessageHandlers
{
    using Commands;
    using Entities;
    using Messages;
    using Repositories;

    public class RemoveBatchCommandHandler : CommandHandler<RemoveBatchCommand>
    {
        private readonly IAppHost host;

        public RemoveBatchCommandHandler(IAppHost host)
        {
            this.host = host;
        }

        public override void Handle(RemoveBatchCommand command)
        {
            var batchRepository = host.GetRequiredService<IRepository<Batch>>();
            var entity = batchRepository.GetByKey(command.EntityID);
            if (entity == null)
            {
                return;
            }
            batchRepository.Remove(entity);
            batchRepository.Context.Commit();

            host.PublishEvent(new BatchRemovedEvent(entity));
            host.CommitEventBus();
        }
    }
}

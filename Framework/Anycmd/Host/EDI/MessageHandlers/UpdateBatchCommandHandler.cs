
namespace Anycmd.Host.EDI.MessageHandlers
{
    using Anycmd.Repositories;
    using Commands;
    using Entities;
    using Exceptions;
    using Messages;

    public class UpdateBatchCommandHandler : CommandHandler<UpdateBatchCommand>
    {
        private readonly AppHost host;

        public UpdateBatchCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(UpdateBatchCommand command)
        {
            var batchRepository = NodeHost.Instance.GetRequiredService<IRepository<Batch>>();
            var entity = batchRepository.GetByKey(command.Input.Id);
            if (entity == null)
            {
                throw new NotExistException();
            }

            entity.Update(command.Input);

            batchRepository.Update(entity);
            batchRepository.Context.Commit();

            host.EventBus.Publish(new BatchUpdatedEvent(entity));
            host.EventBus.Commit();
        }
    }
}

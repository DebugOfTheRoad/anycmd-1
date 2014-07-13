
namespace Anycmd.Host.AC.MessageHandlers
{
    using AC.Infra;
    using Anycmd.Host.AC.Infra.Messages;
    using Anycmd.Repositories;
    using Commands;
    using Exceptions;
    using System;

    public class SaveHelpCommandHandler : CommandHandler<SaveHelpCommand>
    {
        private readonly AppHost host;

        public SaveHelpCommandHandler(AppHost host)
        {
            this.host = host;
        }

        public override void Handle(SaveHelpCommand command)
        {
            var operationHelpRepository = host.GetRequiredService<IRepository<OperationHelp>>();
            var functionRepository = host.GetRequiredService<IRepository<Function>>();
            if (command.FunctionID == Guid.Empty)
            {
                throw new ValidationException("EmptyFunctionID");
            }
            FunctionState operation;
            if (!host.FunctionSet.TryGetFunction(command.FunctionID, out operation))
            {
                throw new ValidationException("没有Id为" + command.FunctionID + "的操作");
            }
            var entity = operationHelpRepository.GetByKey(command.FunctionID);
            bool isNew = false;
            if (entity == null)
            {
                isNew = true;
                entity = new OperationHelp
                {
                    Id = command.FunctionID
                };

            }
            entity.Content = command.Content;
            entity.IsEnabled = command.IsEnabled.HasValue ? command.IsEnabled.Value : 1;
            if (isNew)
            {
                operationHelpRepository.Add(entity);
            }
            else
            {
                operationHelpRepository.Update(entity);
            }
            operationHelpRepository.Context.Commit();
        }
    }
}

using Microsoft.AspNetCore.Http.HttpResults;

namespace Ordering.Api.Application.Commands
{
    public class IdentifiedCommand<T, R> : IRequest<R>
        where T : IRequest<R>
    {
        public IdentifiedCommand(T command, Guid commandId)
        {
            Command = command;
            CommandId = commandId;
        }

        public T Command { get; }
        public Guid CommandId { get; }
    }
}
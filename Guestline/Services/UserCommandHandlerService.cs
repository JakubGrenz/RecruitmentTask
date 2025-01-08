using Guestline.Models;
using Guestline.Models.Commands;

namespace Guestline.Services
{
    public interface IUserCommandHandlerService
    {
        IPrintableCommandResult Handle(Command command);
    }

    public class UserCommandHandlerService: IUserCommandHandlerService
    {
        private readonly IDictionary<Type, Func<Command, IPrintableCommandResult>> _handlers;
        public UserCommandHandlerService(
            ICommandHandlerService<AvailabilityCommand, AvailabilityCommandResult> availabilityHandler,
            ICommandHandlerService<SearchCommand, SearchCommandResult> searchHandler)
        {
            _handlers = new Dictionary<Type, Func<Command, IPrintableCommandResult>>
            {
                { typeof(AvailabilityCommand), cmd => availabilityHandler.Handle((AvailabilityCommand)cmd) },
                { typeof(SearchCommand), cmd => searchHandler.Handle((SearchCommand)cmd) }
            };
        }

        public IPrintableCommandResult Handle(Command command)
        {
            if (_handlers.TryGetValue(command.GetType(), out var handler))
            {
                return handler(command);
            }

            return new ErrorCommandResult
            {
                Error = "Unknown command type"
            };
        }
    }
}

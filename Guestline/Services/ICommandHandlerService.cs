using Guestline.Models;

namespace Guestline.Services
{
    public interface ICommandHandlerService<TCommand, TResult> where TResult: IPrintableCommandResult
    {
        public TResult Handle(TCommand command);
    }
}

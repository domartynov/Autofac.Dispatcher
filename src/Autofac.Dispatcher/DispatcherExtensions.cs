using System.Threading.Tasks;

namespace Autofac.Dispatcher
{
    public static class DispatcherExtensions
    {
        public static TResult Execute<TResult>(this IDispatcher dispatcher, IResultCommand<TResult> command)
        {
            return (TResult) dispatcher.Dispatch(command);
        }

        public static Task Execute(this IDispatcher dispatcher, ICommand command)
        {
            return (dispatcher.Dispatch(command) as Task) ?? Task.FromResult(true);
        }

        public static Task Send(this IDispatcher dispatcher, IEvent @event)
        {
            return (dispatcher.Dispatch(@event) as Task) ?? Task.FromResult(true);
        }
    }
}
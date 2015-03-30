using System;
using System.Threading.Tasks;
using Autofac.Core;

namespace Autofac.Dispatcher
{
    public class Dispatcher : IDispatcher
    {
        private readonly IComponentContext _context;

        public Dispatcher(IComponentContext context)
        {
            _context = context;
        }

        public object Dispatch(object message)
        {
            if (message == null) throw new ArgumentNullException("message");

            return message is IEvent
                ? DispatchEvent(message)
                : DispatchCommand(message);

        }

        private object DispatchEvent(object theEvent)
        {
            var resultTask = new AggregateResultTask();
            var eventType = theEvent.GetType();
            foreach (var r in _context.ComponentRegistry.RegistrationsFor(new HandleService(eventType)))
            {
                foreach (var s in r.Services)
                {
                    var hs = s as HandleService;
                    if (hs != null && hs.MessageType == eventType)
                        resultTask.Add(hs.HandleFunc(r, _context, theEvent) as Task);
                }
            }

            return resultTask.Task;
        }

        private object DispatchCommand(object command)
        {
            var commandType = command.GetType();

            IComponentRegistration r;
            if (!_context.ComponentRegistry.TryGetRegistration(new HandleService(commandType), out r))
                throw new UnresolvedCommandException(commandType);

            foreach (var s in r.Services)
            {
                var hs = s as HandleService;
                if (hs != null && hs.MessageType == commandType)
                    return hs.HandleFunc(r, _context, command);
            }

            return null; // TODO: not expected to be hit, should we do anything about this?
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Core;

namespace Autofac.Dispatcher
{
    public class HandlerRegistrationBuilder<THandler> : IHandlerRegistrationBuilder<THandler>
    {
        private readonly Service _handlerService;
        private List<HandleService> _services;
 
        public IEnumerable<HandleService> HandleServices { get { return _services ?? Enumerable.Empty<HandleService>(); }} 

        public HandlerRegistrationBuilder(Service handlerService)
        {
            _handlerService = handlerService;
        }

        public void For<TMessage>(Func<THandler, IComponentContext, TMessage, Task> handlerFunc)
        {
            Add(typeof(TMessage), (r, ctx, message) => handlerFunc(ResolveComponent(ctx, r), ctx, (TMessage)message));
        }

        public void For<TMessage>(Func<THandler, TMessage, Task> handlerFunc)
        {
            Add(typeof(TMessage), (r, ctx, message) => handlerFunc(ResolveComponent(ctx, r), (TMessage)message));
        }

        public void For<TMessage>(Action<THandler, IComponentContext, TMessage> handlerFunc)
        {
            Add(typeof(TMessage), (r, ctx, message) =>
            {
                handlerFunc(ResolveComponent(ctx, r), ctx, (TMessage)message);
                return Task.FromResult(true);
            });
        }

        public void For<TMessage>(Action<THandler, TMessage> handlerFunc)
        {
            Add(typeof(TMessage), (r, ctx, message) =>
            {
                handlerFunc(ResolveComponent(ctx, r), (TMessage)message);
                return Task.FromResult(true);
            });
        }

        public void For<TCommand, TResult>(Func<THandler, IComponentContext, TCommand, TResult> handlerFunc) where TCommand : IResultCommand<TResult>
        {
            Add(typeof(TCommand), (r, ctx, command) => handlerFunc(ResolveComponent(ctx, r), ctx, (TCommand)command));
        }

        public void For<TCommand, TResult>(Func<THandler, TCommand, TResult> handlerFunc) where TCommand : IResultCommand<TResult>
        {
            Add(typeof(TCommand), (r, ctx, command) => handlerFunc(ResolveComponent(ctx, r), (TCommand)command));
        }

        private static THandler ResolveComponent(IComponentContext context, IComponentRegistration registration)
        {
            return (THandler) context.ResolveComponent(registration, AutofacHelper.NoParameters);
        }

        private void Add(Type messageType, HandleFunc func)
        {
            if (_services == null) _services = new List<HandleService>();
            _services.Add(new HandleService(_handlerService, messageType, func));
        }
    }
}
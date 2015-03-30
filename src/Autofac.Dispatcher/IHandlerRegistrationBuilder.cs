using System;
using System.Threading.Tasks;

namespace Autofac.Dispatcher
{
    public interface IHandlerRegistrationBuilder<THandler>
    {
        void For<TMessage>(Func<THandler, IComponentContext, TMessage, Task> handlerFunc);
        void For<TMessage>(Func<THandler, TMessage, Task> handlerFunc);
        void For<TMessage>(Action<THandler, IComponentContext, TMessage> handlerFunc);
        void For<TMessage>(Action<THandler, TMessage> handlerFunc);
        void For<TCommand, TResult>(Func<THandler, IComponentContext, TCommand, TResult> handlerFunc) where TCommand : IResultCommand<TResult>;
        void For<TCommand, TResult>(Func<THandler, TCommand, TResult> handlerFunc) where TCommand : IResultCommand<TResult>;
    }
}
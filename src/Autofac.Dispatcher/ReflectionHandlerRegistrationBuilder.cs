using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Core;

namespace Autofac.Dispatcher
{
    public class ReflectionHandlerRegistrationBuilder
    {
        private readonly Type _handlerType;
        private readonly Service _handlerService;

        public ReflectionHandlerRegistrationBuilder(Type handlerType, Service handlerService)
        {
            _handlerType = handlerType;
            _handlerService = handlerService;
        }

        public IEnumerable<HandleService> Build()
        {
            return _handlerType.GetMethods().Select(BuildHandleService).Where(NotNull);
        }

        private HandleService BuildHandleService(MethodInfo method)
        {
            if (!method.Name.StartsWith("Handle")) return null;

            var parameters = method.GetParameters();
            if (parameters.Length == 0) return null;

            var messageType = parameters[0].ParameterType;
            return typeof (IEvent).IsAssignableFrom(messageType)
                ? new HandleService(_handlerService, messageType, BuildHandleFunc(method))
                : (typeof (ICommand).IsAssignableFrom(messageType)
                    ? new HandleService(_handlerService, messageType, BuildHandleFunc(method))
                    : null);
        }

        private static HandleFunc BuildHandleFunc(MethodInfo method)
        {
            return (registration, context, command) =>
            {
                var handlerInstance = context.ResolveComponent(registration, AutofacHelper.NoParameters);

                var parameters = method.GetParameters();
                var args = new object[parameters.Length];
                args[0] = command;
                for (var i = 1; i < parameters.Length; i++)
                    args[i] = context.Resolve(parameters[i].ParameterType);

                return method.Invoke(handlerInstance, args);
            };
        }

        private static bool NotNull(object o)
        {
            return o != null;
        }

    }
}
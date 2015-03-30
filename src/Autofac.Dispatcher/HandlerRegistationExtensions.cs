using System;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.Scanning;

namespace Autofac.Dispatcher
{
    public static class HandlerRegistationExtensions
    {
        public static IRegistrationBuilder<THandler, TActivatorData, SingleRegistrationStyle> AsHandler<THandler, TActivatorData>(
            this IRegistrationBuilder<THandler, TActivatorData, SingleRegistrationStyle> rb,
            Action<IHandlerRegistrationBuilder<THandler>> configHandler)
        {
            var handlerTypedService = AddSelfTypedService(rb);
            var hb = new HandlerRegistrationBuilder<THandler>(handlerTypedService);
            configHandler(hb);

            foreach (var hs in hb.HandleServices)
            {
                rb.RegistrationData.AddService(hs);
            }

            return rb;
        }

        public static IRegistrationBuilder<THandler, TActivatorData, SingleRegistrationStyle> AsHandler<THandler, TActivatorData>(
            this IRegistrationBuilder<THandler, TActivatorData, SingleRegistrationStyle> rb, Type handlerType = null)
        {
            var handlerService = AddSelfTypedService(rb, handlerType);
            var handleServices = new ReflectionHandlerRegistrationBuilder(handlerType ?? typeof(THandler), handlerService).Build();

            foreach (var hs in handleServices)
            {
                rb.RegistrationData.AddService(hs);
            }

            return rb;
        }

        public static IRegistrationBuilder<TLimit, TScanningActivatorData, DynamicRegistrationStyle>
            AsHandler<TLimit, TScanningActivatorData>(this IRegistrationBuilder<TLimit, TScanningActivatorData, DynamicRegistrationStyle> registration)
            where TScanningActivatorData : ScanningActivatorData
        {
            if (registration == null) throw new ArgumentNullException("registration");

            registration.ActivatorData.ConfigurationActions.Add((t, rb) =>
            {
                rb.AsHandler(t);
            });

            return registration;
        }

        private static Service AddSelfTypedService<THandler, TActivatorData>(
            IRegistrationBuilder<THandler, TActivatorData, SingleRegistrationStyle> rb, Type handlerType = null)
        {
            var service = new TypedService(handlerType ?? typeof(THandler));
            rb.RegistrationData.AddService(service);

            return service;
        }
    }
}
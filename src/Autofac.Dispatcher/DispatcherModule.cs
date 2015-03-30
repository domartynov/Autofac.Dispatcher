using System;
using System.Collections.Generic;
using Autofac.Core;

namespace Autofac.Dispatcher
{
    public class DispatcherModule : Module
    {
        private readonly HashSet<Type> _commands = new HashSet<Type>(); 

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Dispatcher>().As<IDispatcher>().InstancePerLifetimeScope();
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            foreach (var s in registration.Services)
            {
                var hs = s as HandleService;
                if (hs == null || !typeof (ICommand).IsAssignableFrom(hs.MessageType)) continue;

                if (_commands.Contains(hs.MessageType))
                    throw new DuplicateCommandHandlerRegistrationException(hs.MessageType);
                
                _commands.Add(hs.MessageType);
            }
        }
    }

}

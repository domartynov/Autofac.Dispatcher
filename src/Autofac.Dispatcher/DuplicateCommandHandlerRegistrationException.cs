using System;

namespace Autofac.Dispatcher
{
    public class DuplicateCommandHandlerRegistrationException : Exception
    {
        public DuplicateCommandHandlerRegistrationException(Type commandType)
            : base("Handler for " + commandType + "is already registered.")
        {
        }
    }
}
using System;

namespace Autofac.Dispatcher
{
    public class UnresolvedCommandException : Exception
    
    {
        public UnresolvedCommandException(Type commandType)
            : base(string.Format("Unresolved handler for the {0} command.", commandType))
        {
        }
    }
}
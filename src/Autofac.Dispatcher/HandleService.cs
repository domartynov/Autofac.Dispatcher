using System;
using Autofac.Core;

namespace Autofac.Dispatcher
{
    public delegate object HandleFunc(IComponentRegistration registration, IComponentContext context, object message);

    public class HandleService : Service, IEquatable<HandleService>
    {
        public Service Handler { get; set; }
        public Type MessageType { get; set; }
        public HandleFunc HandleFunc { get; set; }

        public HandleService(Service handler, Type messageType, HandleFunc handleFunc)
        {
            Handler = handler;
            MessageType = messageType;
            HandleFunc = handleFunc;
        }

        public HandleService(Type messageType)
        {
            MessageType = messageType;
        }

        public override string Description
        {
            get { return MessageType.Name + "->" + Handler.Description; }
        }

        public bool Equals(HandleService other)
        {
            if (other == null) return false;

            return MessageType == other.MessageType;
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HandleService);
        }
    }
}
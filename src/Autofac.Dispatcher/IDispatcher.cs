namespace Autofac.Dispatcher
{
    public interface IDispatcher
    {
        object Dispatch(object message);
    }
}
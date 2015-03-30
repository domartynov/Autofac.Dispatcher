namespace Autofac.Dispatcher.Test
{
    public abstract class DispatcherTestBase
    {
        protected readonly ContainerBuilder Builder;

        private IContainer _container;
        protected IContainer Container
        {
            get { return _container ?? (_container = Builder.Build()); }
        }

        protected IDispatcher Dispatcher
        {
            get { return Container.Resolve<IDispatcher>(); }
        }

        protected DispatcherTestBase()
        {
            Builder = new ContainerBuilder();
            Builder.RegisterModule<DispatcherModule>();
        }
    }
}
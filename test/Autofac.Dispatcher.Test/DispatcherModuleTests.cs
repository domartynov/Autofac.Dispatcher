using System;
using FluentAssertions;
using Xunit;

namespace Autofac.Dispatcher.Test
{
    public class DispatcherModuleTests : DispatcherTestBase
    {
        private readonly ContainerBuilder _builder;

        public DispatcherModuleTests()
        {
            _builder = new ContainerBuilder();
            _builder.RegisterModule<DispatcherModule>();
        }

        [Fact]
        public void TestDispatcherIsRegistered()
        {
            var container = _builder.Build();
            var dispatcher = container.ResolveOptional<IDispatcher>();

            dispatcher.Should().NotBeNull();
        }

        [Fact]
        public void TestDispatcherHasLifetimePerScope()
        {
            var container = _builder.Build();
            var scope1 = container.BeginLifetimeScope();
            var scope2 = scope1.BeginLifetimeScope();

            var dispatcher = container.Resolve<IDispatcher>();
            var dispatcher1 = scope1.Resolve<IDispatcher>();
            var dispatcher2 = scope2.Resolve<IDispatcher>();

            dispatcher.Should().NotBeSameAs(dispatcher1);
            dispatcher1.Should().NotBeSameAs(dispatcher2);
            dispatcher2.Should().NotBeSameAs(dispatcher);
        }


        [Fact]
        public void TestRegisterMultipleHandlersForTheSameCommandThrowsAnException()
        {
            _builder.RegisterInstance(new TestHandler1()).AsHandler();
            _builder.RegisterInstance(new TestHandler1()).AsHandler();

            Action buildAction = () => _builder.Build();
            buildAction.ShouldThrow<DuplicateCommandHandlerRegistrationException>();
        }

        public class TestCommand1 : ICommand {  }

        public class TestHandler1
        {
            public void Handle(TestCommand1 command1) { }
        }

        public class TestHandler2
        {
            public IComponentContext HandlerInstanceScope { get; private set; }
            public IComponentContext HandleParameterScope { get; private set; }

            public TestHandler2(IComponentContext handlerInstanceScope)
            {
                HandlerInstanceScope = handlerInstanceScope;
            }

            public void Handle(TestCommand1 command, IComponentContext context)
            {
                HandleParameterScope = context;
            }
        }
    }
}
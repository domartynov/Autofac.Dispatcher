using FluentAssertions;
using Xunit;

namespace Autofac.Dispatcher.Test.Scenarios
{
    public class RegisterHandlerAlongOtherServicesTests : DispatcherTestBase
    {
        [Fact]
        public void HandlerComponentResolvesAdditionalService()
        {
            Builder.RegisterType<Handler1>().AsHandler().As<IService1>().As<IService2>().InstancePerLifetimeScope();

            Dispatcher.Execute(new Command1 {Value = "Test1"});
            var service1 = Container.Resolve<IService1>();
            var service2 = Container.Resolve<IService2>();
            
            service1.Value.Should().Be("Test1");
            service2.Value.Should().Be("Test1");
        }

        [Fact]
        public void HandlerComponentResolvesAdditionalService2()
        {
            Builder.RegisterType<Handler1>().As<IService1>().As<IService2>().AsHandler().InstancePerLifetimeScope();

            Dispatcher.Execute(new Command1 {Value = "Test1"});
            var service1 = Container.Resolve<IService1>();
            var service2 = Container.Resolve<IService1>();
            
            service1.Value.Should().Be("Test1");
            service2.Value.Should().Be("Test1");
        }

        public class Command1 : ICommand
        {
            public string Value { get; set; }
        }

        public interface IService1
        {
            string Value { get; }
        }

        public interface IService2
        {
            string Value { get; }
        }

        public class Handler1 : IService1, IService2
        {
            public string Value { get; set; }

            public void Handle(Command1 command)
            {
                Value = command.Value;
            }
        }

    }
}
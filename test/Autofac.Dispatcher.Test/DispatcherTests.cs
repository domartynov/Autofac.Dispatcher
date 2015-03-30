using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Autofac.Dispatcher.Test
{
    public class DispatcherTests : DispatcherTestBase
    {
        [Fact]
        public void TestDispathcerHasLifetimePerScopeAndUsingACorrespondingScopeToResolveHandlerInstances()
        {
            var activatedInstances = new List<TestHandler1>();
            Builder.RegisterType<TestHandler1>().AsHandler()
                .InstancePerLifetimeScope()
                .OnActivated(args => activatedInstances.Add(args.Instance));

            var scope1 = Container.BeginLifetimeScope();

            scope1.Resolve<IDispatcher>().Send(new TestEvent1());
            Container.Resolve<IDispatcher>().Send(new TestEvent1());


            (from r in activatedInstances select new {r.InstanceScopeTag, r.ParameterScopeTag})
                .ShouldAllBeEquivalentTo(new[]
                {
                    new {InstanceScopeTag = scope1.Tag, ParameterScopeTag = scope1.Tag},
                    new {InstanceScopeTag = Container.Tag, ParameterScopeTag = Container.Tag}
                });
        }

        [Fact]
        public void TestDispatcherHonorsHandlerScopeConfigurationButResolvesParameterDependenciesFromDispatcherContext()
        {
            Builder.RegisterType<TestHandler1>().AsHandler().SingleInstance();

            var scope = Container.BeginLifetimeScope();

            scope.Resolve<IDispatcher>().Send(new TestEvent1());

            var handler = Container.Resolve<TestHandler1>();

            handler.InstanceScopeTag.Should().Be(Container.Tag);
            handler.ParameterScopeTag.Should().Be(scope.Tag);
        }

        [Fact]
        public void TestSendReturnsAggregateTaskOfEventHandler()
        {
            var handler1 = new TestHandler2();
            var handler2 = new TestHandler2();
            Builder.RegisterInstance(handler1).AsHandler();
            Builder.RegisterInstance(handler2).AsHandler();

            var task = Container.Resolve<IDispatcher>().Send(new TestEvent1());
            task.IsCompleted.Should().BeFalse();

            handler1.TaskCompletionSource.SetResult(true);
            task.IsCompleted.Should().BeFalse();

            handler2.TaskCompletionSource.SetCanceled();
            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void TestExecuteCommandReturnsCompeletedTaskForVoidHandler()
        {
            Builder.RegisterType<TestHandler3>().AsHandler();

            var task = Container.Resolve<IDispatcher>().Execute(new TestCommand1());

            task.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void TestExecuteCommandReturnsActualTaskForAsyncHandler()
        {
            var handler = new TestHandler4();
            Builder.RegisterInstance(handler).AsHandler();

            var task = Dispatcher.Execute(new TestCommand1());

            task.IsCompleted.Should().BeFalse();
            handler.TaskCompletionSource.SetResult(true);
            task.IsCompleted.Should().BeTrue();
        }

        public class TestEvent1 : IEvent { }

        public class TestHandler1
        {
            public object InstanceScopeTag { get; private set; }
            public object ParameterScopeTag { get; private set; }

            public TestHandler1(ILifetimeScope handlerInstanceScope)
            {
                InstanceScopeTag = handlerInstanceScope.Tag;
            }

            public void Handle(TestEvent1 command, ILifetimeScope context)
            {
                ParameterScopeTag = context.Tag;
            }
        }

        public class TestHandler2
        {
            public TaskCompletionSource<bool> TaskCompletionSource { get; private set; }

            public TestHandler2()
            {
                TaskCompletionSource = new TaskCompletionSource<bool>();
            }

            public Task Handle(TestEvent1 theEvent)
            {
                return TaskCompletionSource.Task;
            }
        }

        public class TestCommand1 : ICommand { }

        public class TestHandler3
        {
            public void Handle(TestCommand1 command)
            {
                
            }
        }

        public class TestHandler4
        {
            public TaskCompletionSource<bool> TaskCompletionSource { get; private set; }

            public TestHandler4()
            {
                TaskCompletionSource = new TaskCompletionSource<bool>();
            }

            public Task Handle(TestCommand1 command)
            {
                return TaskCompletionSource.Task;
            }
        }
    }
}
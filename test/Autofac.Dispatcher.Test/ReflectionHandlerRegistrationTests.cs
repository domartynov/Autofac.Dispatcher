using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Autofac.Dispatcher.Test
{
    public class ReflectionHandlerRegistrationTests : DispatcherTestBase
    {
        [Fact]
        public void TestCommmand()
        {
            var handler = new TestHandler1();
            Builder.RegisterInstance(handler).AsHandler();

            Dispatcher.Execute(new TestCommand1 {Value = "Test"});

            handler.Value.Should().Be("Test");
        }

        [Fact]
        public void TestResultCommand()
        {
            Builder.RegisterType<TestHandler2>().AsHandler();

            var result = Dispatcher.Execute(new TestCommand2 {Value = "Test"});

            result.Should().Be("Test");
        }

        [Fact]
        public void TestEventDispatcherToAllHandlers()
        {
            var handler1 = new TestHandler3();
            var handler2 = new TestHandler3();
            Builder.RegisterInstance(handler1).AsHandler();
            Builder.RegisterInstance(handler2).AsHandler();

            Dispatcher.Dispatch(new TestEvent1 {Value = "Test"});

            handler1.Value.Should().Be("Test");
            handler2.Value.Should().Be("Test");
        }

        [Fact]
        public void TestScanAssemblyHandlerRegistrationStyle()
        {
            Builder.RegisterAssemblyTypes(typeof(ScanTestHandler1).Assembly)
                .Where(t => t.FullName.StartsWith("Autofac.Dispatcher.Test.ReflectionHandlerRegistrationTests+ScanTestHandler"))
                .AsHandler();

            var testEvent = new ScanTestEvent();
            Dispatcher.Send(testEvent);

            testEvent.HandlerNames.ShouldAllBeEquivalentTo(new[] { "ScanTestHandler1", "ScanTestHandler2" });
        }

        public class TestCommand1 : ICommand
        {
            public string Value { get; set; }
        }

        public class TestHandler1
        {
            public string Value { get; private set; }

            public void Handle(TestCommand1 command)
            {
                Value = command.Value;
            }
        }

        public class TestCommand2 : IResultCommand<string>
        {
            public string Value { get; set; }
        }

        public class TestHandler2
        {
            public string Handle(TestCommand2 command)
            {
                return command.Value;
            }
        }

        public class TestEvent1 : IEvent
        {
            public string Value { get; set; }
        }

        public class TestHandler3
        {
            public string Value { get; set; }

            public void Handle(TestEvent1 theEvent)
            {
                Value = theEvent.Value;
            }
        }

        public class ScanTestEvent : IEvent
        {
            public HashSet<string> HandlerNames { get; private set; }

            public void AddHandler(object handlerInstance)
            {
                (HandlerNames ?? (HandlerNames = new HashSet<string>())).Add(handlerInstance.GetType().Name);
            }
        }

        public class ScanTestHandler1 
        {
            public void Handle(ScanTestEvent theEvent)
            {
                theEvent.AddHandler(this);
            }
        }

        public class ScanTestHandler2
        {
            public void Handle(ScanTestEvent theEvent)
            {
                theEvent.AddHandler(this);
            }
        }
    }
}
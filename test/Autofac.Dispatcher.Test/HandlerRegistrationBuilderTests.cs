using FluentAssertions;
using Xunit;

namespace Autofac.Dispatcher.Test
{
    public class HandlerRegistrationBuilderTests : DispatcherTestBase
    {
        [Fact]
        public void TestVoidCommandHanlder()
        {
            var handler = new TestHandler1();
            Builder.RegisterInstance(handler).AsHandler(hb => hb.For<TestCommand1>((h, c) => h.Handle(c)));

            Dispatcher.Execute(new TestCommand1("Test"));

            handler.Value.Should().Be("Test");
        }

        [Fact]
        public void TestResultCommand()
        {
            Builder.RegisterType<TestHandler1>().AsHandler(hb => hb.For<TestCommand2, string>((h, c) => h.Handle(c)));

            var result = Dispatcher.Execute(new TestCommand2("Test"));

            result.Should().Be("Test");
        }

        [Fact]
        public void TestEventHandler()
        {
            var handler1 = new TestHandler1();
            var handler2 = new TestHandler1();

            Builder.RegisterInstance(handler1).AsHandler(hb => hb.For<TestEvent1>((h, e) => h.Handle(e)));
            Builder.RegisterInstance(handler2).AsHandler(hb => hb.For<TestEvent1>((h, e) => h.Handle(e)));

            Dispatcher.Send(new TestEvent1("Test"));


            handler1.Value.Should().Be("Test");
            handler2.Value.Should().Be("Test");
        }

        public class TestCommand1 : ICommand
        {
            public string Value { get; private set; }

            public TestCommand1(string value)
            {
                Value = value;
            }
        }

        public class TestCommand2 : IResultCommand<string>
        {
            public string Value { get; private set; }

            public TestCommand2(string value)
            {
                Value = value;
            }
        }

        public class TestEvent1 : IEvent
        {
            public string Value { get; private set; }

            public TestEvent1(string value)
            {
                Value = value;
            }
        }

        public class TestHandler1
        {
            public string Value { get; set; }

            public void Handle(TestCommand1 command)
            {
                Value = command.Value;
            }

            public string Handle(TestCommand2 command)
            {
                return command.Value;
            }

            public void Handle(TestEvent1 theEvent)
            {
                Value = theEvent.Value;
            }
        }
    }
}
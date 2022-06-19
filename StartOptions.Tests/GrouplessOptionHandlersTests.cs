using static StartOptions.Tests.Mocks.Applications.MockGrouplessOptionHandlersCommandApplication;
using StartOptions.Tests.Mocks.Applications;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class GrouplessOptionHandlersTests
    {

        [Fact]
        public void TestWithoutHandlers()
        {
            var app = this.GetApplication(HandlingMode.None);
            app.Run(new string[] { "-v", "-n=1,2,3", "-w=test,text,flex", "-motd=Just a message" });

            this.AssertValues(app, false, null, null, null);
        }

        [Fact]
        public void TestWithHandlers()
        {
            var app = this.GetApplication(HandlingMode.Correct);
            
            app.Run(new string[] { "-v" });
            this.AssertValues(app, true, null, null, null);

            app.Run(new string[] { "-n=1,2,3" });
            this.AssertValues(app, true, new int[] { 1, 2, 3 }, null, null);

            app.Run(new string[] { "-w=test,text,flex" });
            this.AssertValues(app, true, new int[] { 1, 2, 3 }, new string[] { "test", "text", "flex" }, null);

            app.Run(new string[] { "-motd=Just a message" });
            this.AssertValues(app, true, new int[] { 1, 2, 3 }, new string[] { "test", "text", "flex" }, "Just a message");

            app.Run(new string[] { "-w=word" });
            this.AssertValues(app, true, new int[] { 1, 2, 3 }, new string[] { "word" }, "Just a message");

            app.Run(new string[] { "-n=1" });
            this.AssertValues(app, true, new int[] { 1 }, new string[] { "word" }, "Just a message");
        }

        [Fact]
        public void TestHandlerExceptions()
        {
            var app = this.GetApplication(HandlingMode.Incorrect);

            Assert.Throws<ArgumentException>(() => app.Run(new string[] { "-n=test,text,flex" }));
            Assert.Throws<ArgumentException>(() => app.Run(new string[] { "-motd=1" }));
            Assert.Throws<ArgumentException>(() => app.Run(new string[] { "-w=1,2,3" }));
        }

        [Fact]
        public void TestAttachDuplicateHandler()
        {
            var app = this.GetApplication(HandlingMode.Correct);

            Assert.Throws<InvalidOperationException>(() => app.AddGlobalGrouplessStartOptionHandler("verbose", () => { }));
        }

        private void AssertValues(MockGrouplessOptionHandlersCommandApplication app, bool verbose, int[] numbers, string[] words, string motd)
        {
            AssertionUtility.Array(numbers, app.Numbers);
            AssertionUtility.Array(words, app.Words);
            Assert.Equal(motd, app.MessageOfTheDay);
            Assert.Equal(verbose, app.Verbose);
        }

        private MockGrouplessOptionHandlersCommandApplication GetApplication(HandlingMode mode)
        {
            return new MockGrouplessOptionHandlersCommandApplication(mode);
        }
    }
}

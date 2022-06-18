using StartOptions.Tests.Mocks.Applications;
using LunarDoggo.StartOptions.Exceptions;
using StartOptions.Tests.Mocks;
using LunarDoggo.StartOptions;
using System.Linq;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class ApplicationTests
    {
        [Fact]
        public void TestRunWithoutGroup()
        {
            string[] args = new string[] { "-d", "-v" };

            foreach(IMockApplication app in this.GetApplications())
            {
                app.Run(args);

                Assert.Null(app.Group);
                Assert.True(app.GrouplessStartOptions.Count() == 2);

                StartOption verbose = app.GrouplessStartOptions.Single(_o => _o.LongName.Equals("verbose"));
                StartOption debug = app.GrouplessStartOptions.Single(_o => _o.LongName.Equals("debug"));
                AssertionUtility.StartOption("verbose", "v", null, false, StartOptionValueType.Switch, verbose);
                AssertionUtility.StartOption("debug", "d", null, false, StartOptionValueType.Switch, debug);
            }
        }

        [Fact]
        public void TestRunWithoutAnyStartOptions()
        {
            string[] args = new string[] { };

            foreach(IMockApplication app in this.GetApplications())
            {
                //Should just run, as no group is required 
                app.Run(args);
                Assert.Null(app.Group);
            }

            foreach(IMockApplication app in this.GetApplications(true))
            {
                Assert.Throws<OptionRequirementException>(() => app.Run(args));
            }
        }

        [Fact]
        public void TestRunWithoutGrouplessOptions()
        {
            string[] args = new string[] { "-a", "-1", "10", "-2", "15" };

            foreach(IMockApplication app in this.GetApplications())
            {
                app.Run(args);

                Assert.Empty(app.GrouplessStartOptions);
                Assert.NotNull(app.Group);
                AssertionUtility.StartOptionGroup("add", "a", null, app.Group);
            }
        }
        
        [Fact]
        public void TestRunWithTwoGroups()
        {
            string[] args = new string[] { "-d", "-v", "-a", "-1", "5", "-2", "10", "-s", "--number1", "10", "--number2", "2" };
            
            foreach(IMockApplication app in this.GetApplications())
            {
                Assert.Throws<InvalidOperationException>(() => app.Run(args));
            }
        }

        private IMockApplication[] GetApplications(bool requireGroup = false)
        {
            return new IMockApplication[]
            {
                new MockBuilderApplication(requireGroup),
                new MockCommandApplication(requireGroup)
            };
        }
    }
}
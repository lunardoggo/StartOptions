using LunarDoggo.StartOptions.Parsing.Values;
using LunarDoggo.StartOptions.Building;
using LunarDoggo.StartOptions.Parsing;
using System.Collections.Generic;
using LunarDoggo.StartOptions;
using System.Diagnostics;
using System;
using Xunit;

namespace StartOptions.Tests
{
    public class StartOptionParserPerformanceTests
    {
        [Fact]
        public void TestSmallStartOptionParser()
        {
            StartOptionGroup[] groups = this.GetSmallStartOptionGroupArray();
            StartOption[] options = this.GetSmallGrouplessStartOptionArray();
            string[] args = new string[] { "-?", "-g", "--option", "-v" };

            this.AssertParsingIsFasterThanMilliseconds(50, groups, options, args);
        }

        private StartOptionGroup[] GetSmallStartOptionGroupArray()
        {
            return new[]
            {
                new StartOptionGroupBuilder("group", "g")
                    .SetDescription("The only available start option group")
                    .AddOption("option", "o", (_builder) => _builder.SetDescription("Option of the group").SetRequired())
                    .Build()
            };
        }

        private StartOption[] GetSmallGrouplessStartOptionArray()
        {
            return new[]
            {
                new StartOptionBuilder("verbose", "v").SetDescription("Enable verbose output").Build()
            };
        }

        [Fact]
        public void TestBigStartOptionParser()
        {
            StartOptionGroup[] groups = this.GetBigStartOptionGroupArray();
            StartOption[] options = this.GetBigGrouplessStartOptionArray();
            string[] args = new string[] { "-?", "-i", "--path=./import.sql", "--port=3306", "--server=127.0.0.1", "-t=users", "-f", "-v", "-u=root", "-s=dba" };

            this.AssertParsingIsFasterThanMilliseconds(200, groups, options, args);
        }

        private StartOptionGroup[] GetBigStartOptionGroupArray()
        {
            return new[]
            {
                new StartOptionGroupBuilder("import", "i")
                    .SetDescription("Imports a file to a database table")
                    .AddOption("path", "p", (_builder) => _builder.SetDescription("Path to the imported file").SetValueType(StartOptionValueType.Single).SetRequired())
                    .AddOption("table", "t", (_builder) => _builder.SetDescription("Name of the table").SetValueType(StartOptionValueType.Single).SetRequired())
                    .AddOption("force", "f", (_builder) => _builder.SetDescription("Overwrite already existing records in the table"))
                    .Build(),
                new StartOptionGroupBuilder("export", "e")
                    .SetDescription("Exports a database table to a file")
                    .AddOption("path", "p", (_builder) => _builder.SetDescription("Path to the exported file").SetValueType(StartOptionValueType.Single).SetRequired())
                    .AddOption("table", "t", (_builder) => _builder.SetDescription("Name of the table").SetValueType(StartOptionValueType.Single).SetRequired())
                    .Build(),
                new StartOptionGroupBuilder("show-records", "r")
                    .SetDescription("Shows all records in the specified table")
                    .AddOption("table", "t", (_builder) => _builder.SetDescription("Name of the table").SetValueType(StartOptionValueType.Single).SetRequired())
                    .Build(),
                new StartOptionGroupBuilder("clear-records", "c")
                    .SetDescription("Clears all records from the specified table")
                    .AddOption("table", "t", (_builder) => _builder.SetDescription("Name of the table").SetValueType(StartOptionValueType.Single).SetRequired())
                    .Build(),
                new StartOptionGroupBuilder("drop-table", "d")
                    .SetDescription("Drops the table with the specified name")
                    .AddOption("table", "t", (_builder) => _builder.SetDescription("Name of the table").SetValueType(StartOptionValueType.Single).SetRequired())
                    .Build(),
                new StartOptionGroupBuilder("list", "l")
                    .SetDescription("Lists all tables in the database")
                    .Build()
            };
        }

        private StartOption[] GetBigGrouplessStartOptionArray()
        {
            return new[]
            {
                new StartOptionBuilder("port", "po").SetDescription("Port of the sql server").SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()).Build(),
                new StartOptionBuilder("server", "srv").SetDescription("Address of the sql server").SetValueType(StartOptionValueType.Single).SetRequired().Build(),
                new StartOptionBuilder("username", "u").SetDescription("Name of the database user").SetValueType(StartOptionValueType.Single).SetRequired().Build(),
                new StartOptionBuilder("password", "pass").SetDescription("Password of the database user").SetValueType(StartOptionValueType.Single).Build(),
                new StartOptionBuilder("schema", "s").SetDescription("Name of the sql schema").SetValueType(StartOptionValueType.Single).Build(),
                new StartOptionBuilder("verbose", "v").SetDescription("Enable verbose output").Build()
            };
        }

        private void AssertParsingIsFasterThanMilliseconds(long milliseconds, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> options, string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            new StartOptionParser(groups, options, StartOptionParser.DefaultHelpOptions).Parse(args);
            sw.Stop();

            string methodName = new StackTrace().GetFrame(1).GetMethod().Name;
            Console.WriteLine($"Performance Test \"{methodName}\" finished after {sw.ElapsedMilliseconds} ms");
            Assert.True(sw.ElapsedMilliseconds < milliseconds);
        }
    }
}

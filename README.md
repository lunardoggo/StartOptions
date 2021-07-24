# StartOptions
Library for parsing commandline start options for .net and .net-core applications (.net-standard 1.3)
---
# Terminology
A `StartOption` is a single commandline argument that may contain values. StartOptions can either be grouped by StartOptionGroups or be groupless and can therefore be used for global flags like `verbose` or `debug`. StartOptions can be built using a `StartOptionBuilder` with the following methods:

    new StartOptionBuilder(string longName, string shortName)
            .SetValueParser(IStartOptionValueParser valueParser) //Valueparsers parse the value string of the argument into a desired type
            .SetValueType(StartOptionValueType type) //Sets the type of the option Switch = no value, Single = one value, Multiple = multiple values separated by the configured separator (default: ",")
            .SetDescription(string description) //Description that is displayed on the help-page
            .SetRequired(bool required = true) //Specify if the StartOption must be set on the commandline
            .Build(); //Create a instance of StartOption from the previously chained parameters
            
A `StartOptionGroup` is a grouping of StartOptions and has a specific name. The StartOptionGroup-commandline-argument must not contain a value, a StartOptionGroup requires at least one subordinate StartOption to be defined. Also note that you can only use one StartOptionGroup at once when calling your application. StartOptionGroups can be built using a `StartOptionBuilder` with the following methods:

    new StartOptionGroupBuilder(string longName, string shortName)
            .AddOption(string longName, string shortName, Action<StartOptionBuilder> buildAction) //Add a subordinage StartOption to the group; in buildAction you can use all methods of StartOptionBuilder except StartOptionBuilder.Build() (see example below)
            .AddOption(StartOption option) //Alternative to the other StartOptionGroupBuilder.AddOption-method if you built your groups StartOptions beforehand
            .SetDescription(string description) //Description that is displayed on the help-page
            .Build(); //Create a instance of StartOptionGroup from the previously chained parameters

A `StartOptionParser` takes the commandline arguments and parses them into an optional `StartOptionGroup` and `StartOption`s which you can use to determine the inputs from the cli. A StartOptionBuilder can be built using one of its constructors, if you use `AbstractApplication`, you don't need to use this class:

    StartOptionParser parser = new StartOptionParser(StartOptionParserSettings settings, //Your customized parser settings, for default settings just use new StartOptionParserSettings() 
                                                     IEnumerable<StartOptionGroup> groups, //All StartOptionGroups your application will be able to distinguish; can be null
                                                     IEnumerable<StartOption> grouplessOptions, //All StartOptions that aren't grouped; can be null
                                                     IEnumerable<HelpOption> helpOptions); //All HelpOptions which will set the HelpRequested-flag in the parser-output, for default use StartOptionParser.DefaultHelpOptions
    parser.Parse(string[] args); //Input your commandline arguments from your Main-method
                         

You can also use a Simpler constructor of StartOptionParser which will use the default parser settings and help options:

    StartOptionParser(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)

An `AbstractApplication` is a base type that creates a StartOptionParser, passes your applications commandline arguments to it and handles its output. You must override the following methods:

    protected abstract void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions); //Print your help-page if the commandline arguments containes a HelpOption
    protected abstract void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions); //Run your application code with the parsed StartOptionGroup and groupless StartOptions

    protected abstract ApplicationStartOptions GetApplicationStartOptions(); //Get the ApplicationStartOptions for the StartOptionParser-creation

`ApplicationStartOptions` is a container used by an `AbstractApplication` in order to provide the `StartOptionParser` with its parameters. It is just a simple container for StartOptionGroups, groupless StartOptions, HelpOptions and StartOptionParserSettings with the following constructors:

    public ApplicationStartOptions(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions, IEnumerable<HelpOption> helpOptions, StartOptionParserSettings parserSettings)
    public ApplicationStartOptions(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions, IEnumerable<HelpOption> helpOptions)
    public ApplicationStartOptions(IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
---
# Usage
In order to use the easiest interface to this library, create a new class that inherits from `AbstractApplication` and override its abstract methods:
    
    class DemoApplication : AbstractApplication
    
Set your ApplicationStartOptions inside of `GetApplicationStartOptions`, note that valid StartOption names must start with either a letter or a number and can only contain letters, numbers, underscores and hyphens/dashes. Also note, that you can also provide custom HelpOptions and StartOptionParserSettings in this method (see Demo-Project):

    protected override ApplicationStartOptions GetApplicationStartOptions()
    {
        StartOptionGroup[] groups = new StartOptionGroup[]
        {
            new StartOptionGroupBuilder("add", "a").SetDescription("Adds two integers together")
                    .AddOption("value-1", "1", (_option) => _option.SetDescription("First value").SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()).SetRequired())
                    .AddOption("value-2", "2", (_option) => _option.SetDescription("Second value").SetValueType(StartOptionValueType.Single).SetValueParser(new Int32OptionValueParser()).SetRequired())
                    .Build()
        };
        
        StartOption[] grouplessOptions = new StartOption[]
        {
            new StartOptionBuilder("verbose", "v").SetDescription("Enable verbose output").Build()
        };
        
        return new ApplicationStartOptions(groups, grouplessOptions);
    }
    
Set the logic for printing help-pages inside of `PrintHelpPage`. You can use the predefined class `ConsoleHelpPrinter` or define your own method of displaying a help page to your user:

    protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
    {
        new ConsoleHelpPrinter('\t').Print(settings, helpOptions, groups, grouplessOptions);
    }
    
Set the logic of the execution of your application inside of `Run`:

    protected override void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions)
    {
        if(selectedGrouplessOptions.Any(_option => _option.ShortName.Equals("v")))
        {
            Console.WriteLine("Verbose option was toggled");
        }

        switch(selectedGroup.ShortName)
        {
            case "a":
                StartOption firstOption = group.GetOptionByShortName("1");
                StartOption secondOption = group.GetOptionByShortName("2");

                if (firstOption.HasValue && secondOption.HasValue)
                {
                    int first = firstOption.GetValue<int>();
                    int second = secondOption.GetValue<int>();

                    Console.WriteLine("{0} + {1} = {2}", first, second, first + second);
                }
                else
                {
                    if (!firstOption.HasValue)
                    {
                        Console.WriteLine("Please provide the first number for the addition");
                    }
                    if(!secondOption.HasValue)
                    {
                        Console.WriteLine("Please provide the second number for the addition");
                    }
                }
                break;
            default: throw new NotImplementedException();
        }
    }

Call your application inside your Main-method:

    static void Main(string[] args)
    {
        DemoApplication application = new DemoApplication();
        application.Run(args);
    }

Call your application from the commandline, eg.:

    /> .\DemoApplication.exe --add -1=10 -2=5 --verbose -h

In this example, your application will be using the "add"-StartOptionGroup with the subordinate StartOptions "value-1" (value = 10) and "value-2" (value = 5), the verbose-flag is set. As the HelpOption "h" is also used, your application will display the help-page for the provided commandline arguments without actually executing the operation, in order to run the addition, just omit the "-h" option.

Please note, that the help printer by default only displays all options if your commandline arguments only contained HelpOptions if they contained StartOptions or StartOptionGroups, the help page will only contain descriptions to the provided options. To change this behaviour, override the following Method in your Application-class:
    
    protected override void PrintHelpPage(ParsedStartOptions parsed)
    {
        //The following four lines assume, that you defined methods for getting your StartOptionParserSettings, StartOptionGroups, StartOptions and HelpOptions
        StartOptionParserSettings settings = this.GetStartOptionParserSettings();
        IEnumerable<StartOptionGroup> groups = this.GetStartOptionGroups();
        IEnumerable<HelpOption> helpOptions = this.getHelpOptions();
        IEnumerable<StartOption> options = this.GetStartOptions();

        this.PrintHelpPage(settings, helpOptions, groups, options);
    }
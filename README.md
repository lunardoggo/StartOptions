# StartOptions
Library for parsing commandline start options for .net and .net-core applications (.net-standard 1.3)
---
# Terminology
A [StartOption](https://github.com/lunardoggo/StartOptions/wiki/StartOption) is a single commandline argument that may contain values. StartOptions can either be grouped by StartOptionGroups or be groupless and can therefore be used for global flags, such as `verbose` or `debug`.

A [StartOptionGroup](https://github.com/lunardoggo/StartOptions/wiki/StartOptionGroup) is a grouping of `StartOption`s, it has a specific name associated with it. The StartOptionGroup commandline argument always is of type `Switch`, it additionally requires at least one subordinate StartOption to be defined. Also note that you can only use one StartOptionGroup at once when calling your application.

A [StartOptionParser](https://github.com/lunardoggo/StartOptions/wiki/StartOptionParser) takes the commandline arguments and parses them into an optional `StartOptionGroup` and `StartOption`s which you can use to determine the inputs from the cli. If you use either of the two approaches mentioned in the section **Getting started** you don't need to use this class.

[AbstractApplication](https://github.com/lunardoggo/StartOptions/wiki/AbstractApplication) is a base type that creates a StartOptionParser, passes your application's commandline arguments to it and handles its output using the builder-approach mentioned below. You must override the following methods:
```csharp
//Prints your help-page if the commandline arguments containes a HelpOption
protected abstract void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions);

//Runs your application code with the parsed StartOptionGroup and groupless StartOptions
protected abstract void Run(StartOptionGroup selectedGroup, IEnumerable<StartOption> selectedGrouplessOptions);

//Gets the ApplicationStartOptions for the StartOptionParser-creation
protected abstract ApplicationStartOptions GetApplicationStartOptions();
```
[CommandApplication](https://github.com/lunardoggo/StartOptions/wiki/CommandApplication) is a base type that does the same thing as `AbstractApplication`, but with the attribute-based approach mentioned below. You must override the following methods:
```csharp
//Prints your help-page if the commandline arguments containes a HelpOption
protected abstract void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions);

//Get the types of all commands your appllication supports
protected abstract Type[] GetCommandTypes();
```
[ApplicationStartOptions](https://github.com/lunardoggo/StartOptions/wiki/ApplicationStartOptions) is a container used by an `AbstractApplication` or a `CommandApplication` in order to provide the `StartOptionParser` with its parameters. It is just a simple container for `StartOptionGroups`, groupless `StartOptions`, `HelpOptions` and [StartOptionParserSettings](https://github.com/lunardoggo/StartOptions/wiki/StartOptionParserSettings)

---

# Getting started
The first step to using this library is to install the lates version of the [nuget package](https://www.nuget.org/packages/LunarDoggo.StartOptions/) for your project,
for example by using the package manager console in Visual Studio:
```powershell
Install-Package LunarDoggo.StartOptions [-ProjectName <your project name>]
```

This library provides two distinct ways for customizing your application's command line arguments:
  * by utilizing attributes
  * by building the StartOptions yourself

### 1. Using the attribute-based approach
If you chose the attribute-based approach, first create some classes that implement the interface `IApplicationCommand`, make sure to define at least one constructor per class that is decorated with the [StartOptionGroupAttribute](https://github.com/lunardoggo/StartOptions/wiki/StartOptionGroupAttribute) and contains parameters that are decorated with the [StartOptionAttribute](https://github.com/lunardoggo/StartOptions/wiki/StartOptionAttribute). If you want to add constructor parameters that aren't decorated with a StartOptionAttribute, you have to provide an [IDependencyProvider](https://github.com/lunardoggo/StartOptions/wiki/DependencyProvider). The method `Execute()` contains the code that will run the command:
```csharp
public class AddCommand : IApplicationCommand
{
    private readonly int firstValue, secondValue;

    [StartOptionGroup("add", "a", Description = "Adds two integers together")]
    public AddCommand([StartOption("value-1", "1", Description = "First value", Mandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int firstValue,
                        [StartOption("value-2", "2", Description = "Second value", Mandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int secondValue)
    {
        this.secondValue = secondValue;
        this.firstValue = firstValue;
    }

    public void Execute()
    {
        Console.WriteLine("{0} + {1} = {2}", this.firstValue, this.secondValue, this.firstValue + this.secondValue);
    }
}
```
**Note**: See **Final notes and hints** below to learn how to implement groupless StartOptions.

After you created all the commands you want to use, create a new class that inherits from `CommandApplication` and override its abstract methods:
```csharp
class DemoApplication : CommandApplication
```
Set the commands the application will be able to run, all available command line arguments will be extracted from:
```csharp
protected override Type[] GetCommandTypes()
{
    return new[] { typeof(AddCommand), typeof(ReadFileCommand) };
}
```
Set the [IDependencyProvider](https://github.com/lunardoggo/StartOptions/wiki/DependencyProvider) to be used to resolve constructor parameters that aren't decorated with a StartOptionAttribute. If you don't want to use this feature, simply return `null`. The library is shipped with the [SimpleDependencyProvider](https://github.com/lunardoggo/StartOptions/wiki/DependencyProvider), you can also use dependency injection frameworks, like Ninject, but you'll have to implement a proxy class implementing `IDependencyProvider` to allow access to the dependency framework. This example will use the SimpleDependencyProvider:
```csharp
protected override IDependencyProvider GetDependencyProvider()
{
    //true: throw an exception if a dependency can't be resolved
    SimpleDependencyProvider provider = new SimpleDependencyProvider(true);
    //Add dependencies to the cache like so, keep in mind that every Type can only be registered once:
    provider.AddSingleton<IDatabase>(new MySqlDatabase("connection string here"));
    return provider;
}
```
And finally implement the method that will print the help page of your application, you can implement a custom help page printing mechanism or simply use the included class [ConsoleHelpPrinter](https://github.com/lunardoggo/StartOptions/wiki/IHelpPagePrinter):
```csharp
protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
{
    new ConsoleHelpPrinter('\t').Print(settings, helpOptions, groups, grouplessOptions);
}
```
### 2. Using StartOptionGroup- and StartOption-builders
If you picked the builder-based approach, create a new class that inherits from `AbstractApplication` and override its abstract methods:
```csharp
class DemoApplication : AbstractApplication
```
Set your ApplicationStartOptions inside of `GetApplicationStartOptions`, note that valid StartOption names must start with either a letter or a number and can only contain letters, numbers, underscores and hyphens/dashes. Also note, that you can also provide custom HelpOptions and StartOptionParserSettings in this method (see Demo-Project):
```csharp
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
```
Set the logic for printing help-pages inside of `PrintHelpPage`. You can use the predefined class `ConsoleHelpPrinter` or define your own method of displaying a help page to your user:
```csharp
protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
{
    new ConsoleHelpPrinter('\t').Print(settings, helpOptions, groups, grouplessOptions);
}
```
Set the logic of the execution of your application inside of `Run`:
```csharp
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
```
---
Regardless of whether you chose the attribute-based or builder-based approach, you must finally instantiate your application class in the `Main(string[] args)`-method and call the `Run(string[] args)`-method on your application class:
```csharp
static void Main(string[] args)
{
    DemoApplication application = new DemoApplication();
    application.Run(args);
}
```
Afterwards you will be able to call your application from the commandline like so:

    /> .\DemoApplication.exe --add -1=10 -2=5 --verbose -h

In this example, your application will be using the "add"-StartOptionGroup with the subordinate StartOptions "value-1" (value = 10) and "value-2" (value = 5), the verbose-flag is set. As the HelpOption "h" is also used, your application will display the help-page for the provided commandline arguments without actually executing the operation, in order to run the addition, just omit the "-h" option.

---

### Final notes and hints
1. Please note, that the help printer by default only displays all options if your commandline arguments only contained HelpOptions if they contained StartOptions or StartOptionGroups, the help page will only contain descriptions to the provided options. To change this behaviour, override the following Method in your Application-class:
```csharp
protected override void PrintHelpPage(ParsedStartOptions parsed)
{
    //The following four lines assume, that you defined methods for getting your StartOptionParserSettings, StartOptionGroups, StartOptions and HelpOptions
    StartOptionParserSettings settings = this.GetStartOptionParserSettings();
    IEnumerable<StartOptionGroup> groups = this.GetStartOptionGroups();
    IEnumerable<HelpOption> helpOptions = this.getHelpOptions();
    IEnumerable<StartOption> options = this.GetStartOptions();

    this.PrintHelpPage(settings, helpOptions, groups, options);
}
```
2. If you wish to parse command line arguments in the **command-based approach** that are not of type `string`, you must specify the parser a `StartOption` should use to parse its values:
```csharp
[StartOptionGroup("add", "a", Description = "Adds two integers together")]
public AddCommand([StartOption("value-1", "1", Description = "First value", Mandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int firstValue,
                    [StartOption("value-2", "2", Description = "Second value", Mandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int secondValue)
{
    this.secondValue = secondValue;
    this.firstValue = firstValue;
}
```
3. If you wish to implement a custom `IStartOptionValueParser` with the **command-based approach** you must register your custom parser in the `StartOptionValueParserRegistry` before using it:
```csharp
class CustomOptionValueParser : AbstractStartOptionValueParser
{
    ...
}

static void Main(string[] args)
{
    StartOptionValueParserRegistry.Register(new CustomOptionValueParser());
    DemoApplication application = new DemoApplication();
    application.Run(args);
}
```
4. If you wish to use the same groupless `StartOption` with the **command-based approach** across multiple different constructors or classes, at first implement one command with a constructor parameter decorated with `GrouplessStartOptionAttribute` and another one (another constructor in the same or a different class) decorated with `GrouplessStartOptionReferenceAttribute`. Make sure that no groupless start options use duplicate names:
```csharp
class FirstCommand : ApplicationCommand
{
    [StartOptionGroup("first", "f")]
    public FirstCommand(..., [GrouplessStartOption("verbose", "v", Description = "Enable verbose output", IsGrouplessOption = true)]bool verbose)
    {
        ...
    }
    ...
}

class SecondCommand : ApplicationCommand
{
    [StartOptionGroup("second", "s")]
    public SecondCommand(..., [GrouplessStartOptionReference("verbose")]bool verbose)
    {
        ...
    }
    ...
}
```

4. 1 You can also decorate your `CommandApplication` or any of your `IApplicationCommand` implementations with `GrouplessStartOptionAttribute` if you prefer your grouless StartOptions to be declared in a central location:
```csharp
[GrouplessStartOption("verbose", "v", Description = "Enable verbose output")]
public class DemoApplication : CommandApplication
{
    ...
}

[GrouplessStartOption("debug", "d", Description = "Enable debug mode")]
public class DemoCommand : IApplicationCommand
{
    public DemoCommand(..., [GrouplessStartOptionReference("verbose")]bool verbose,
                       [GrouplessStartOptionReference("debug")]bool debug)
   {
       ...
   }
    ...
}
```

5. If you want to handle groupless StartOptions of your `CommandApplication` in a central location, you can attach handlers to your application (note: right now you can only attach one handler per groupless StartOption) by using these methods either in your application's constructor or from outside your application class:
```csharp
CommandApplication app = new DemoApplication();
//for groupless StartOptions of type Multiple
app.AddGlobalGrouplessStartOptionHandler<T>("numbers", (int[] _numbers) => this.Numbers = _numbers);
//for groupless StartOptions of type Single
app.AddGlobalGrouplessStartOptionHandler<T>("user", (string _username) => this.Username = _username);
//for groupless StartOptions of type Switch
app.AddGlobalGrouplessStartOptionHandler("verbose", () => this.Verbose = true);
```

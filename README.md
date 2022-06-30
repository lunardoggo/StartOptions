LunarDoggo.StartOptions
============
[![License](https://img.shields.io/github/license/LunarDoggo/StartOptions)](https://github.com/lunardoggo/StartOptions/blob/main/LICENSE)
[![Nuget](https://img.shields.io/nuget/vpre/LunarDoggo.StartOptions)](https://www.nuget.org/packages/LunarDoggo.StartOptions/)
![Nuget](https://img.shields.io/nuget/dt/LunarDoggo.StartOptions)

Library for parsing commandline arguments into much more managable [StartOptions](https://github.com/lunardoggo/StartOptions/wiki/StartOption) and [StartOptionGroups](https://github.com/lunardoggo/StartOptions/wiki/StartOptionGroup) for .net and .net-core applications (.net-standard 1.3). For detailed information take a look this repository's [wiki](https://github.com/lunardoggo/StartOptions/wiki).
---

# Usage
The first step to using this library is to install the lates version of the [nuget package](https://www.nuget.org/packages/LunarDoggo.StartOptions/) for your project,
for example by using the package manager console in Visual Studio:
```powershell
Install-Package LunarDoggo.StartOptions [-ProjectName <your project name>]
```

This library provides two distinct ways for customizing your application's command line arguments:
  * by utilizing attributes
  * by building the StartOptions yourself

### [1. Using the attribute-based approach](https://github.com/lunardoggo/StartOptions/tree/main/Demos/StartOptions.Demo.Commands)
At first create some classes that implement the interface `IApplicationCommand`, make sure to define at least one constructor per class that is decorated with the [StartOptionGroupAttribute](https://github.com/lunardoggo/StartOptions/wiki/StartOptionGroupAttribute) and contains parameters that are decorated with the
[StartOptionGroupValueAttribute](https://github.com/lunardoggo/StartOptions/wiki/StartOptionGroupValueAttribute), [StartOptionAttribute](https://github.com/lunardoggo/StartOptions/wiki/StartOptionAttribute), [GrouplessStartOptionAttribute](https://github.com/lunardoggo/StartOptions/wiki/GrouplessStartOptionAttribute) or [GrouplessStartOptionReferenceAttribute](https://github.com/lunardoggo/StartOptions/wiki/GrouplessStartOptionAttributeReference). If you want to add constructor parameters that aren't decorated with any of these attributes, you have to provide an [IDependencyProvider](https://github.com/lunardoggo/StartOptions/wiki/DependencyProvider). The method `Execute()` contains the code that will run the command:
```csharp
public class AddCommand : IApplicationCommand
{
    private readonly int firstValue, secondValue;

    [StartOptionGroup("add", "a", Description = "Adds two integers together")]
    public AddCommand([StartOption("value-1", "1", Description = "First value", Mandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int firstValue,
                        [StartOption("value-2", "2", Description = "Second value", Mandatory = true, ValueType = StartOptionValueType.Single, ParserType = typeof(Int32OptionValueParser))] int secondValue,
                        [GrouplessStartOption("verbose", "v")]bool verbose)
    {
        this.secondValue = secondValue;
        this.firstValue = firstValue;
        //do something with the verbose switch
    }

    public void Execute()
    {
        Console.WriteLine("{0} + {1} = {2}", this.firstValue, this.secondValue, this.firstValue + this.secondValue);
    }
}
```
**Note:** If you want to use the same groupless `StartOption` across multiple constructors/commands, take a look at the section `Final notes and hints` below.

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
### [2. Using StartOptionGroup- and StartOption-builders](https://github.com/lunardoggo/StartOptions/tree/main/Demos/StartOptions.Demo.Builders)
If you picked the builder-based approach, create a new class that inherits from `AbstractApplication` and override its abstract methods:
```csharp
class DemoApplication : AbstractApplication
```
Set your ApplicationStartOptions inside of `GetApplicationStartOptions`, note that valid StartOption names must start with either a letter or a number and can only contain letters, numbers, underscores and hyphens/dashes. Also note, that you can also provide custom HelpOptions and StartOptionParserSettings in this method (see [Demo-Project]((https://github.com/lunardoggo/StartOptions/tree/main/Demos/StartOptions.Demo.Builders))):
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
Implement the logic for printing help-pages inside of `PrintHelpPage`. You can use the predefined class `ConsoleHelpPrinter` as shown below or define your own method of displaying a help page to your users:
```csharp
protected override void PrintHelpPage(StartOptionParserSettings settings, IEnumerable<HelpOption> helpOptions, IEnumerable<StartOptionGroup> groups, IEnumerable<StartOption> grouplessOptions)
{
    new ConsoleHelpPrinter('\t').Print(settings, helpOptions, groups, grouplessOptions);
}
```
Implement the logic of the execution of your application inside of `Run`:
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

### Parsing values of any type
By default your `StartOptions` will parse arguments as `strings`. If you wish to parse the provided command line argument into another type, you have to specify the parser type as shown above. The library ships with these predefined parsers:
  * `BoolOptionValueParser`
  * `ByteOptionValueParser`
  * `DoubleOptionValueParser`
  * `FloatOptionValueParser`
  * `Int16OptionValueParser`
  * `Int32OptionValueParser`
  * `Int64OptionValueParser`

**Note:** `FloatOptionValueParser` as well as `DoubleOptionValueParser` use the current system culture as the source of the floating point number format.

You can create custom [IStartOptionValueParsers](https://github.com/lunardoggo/StartOptions/wiki/IStartOptionValueParser) for any type you like by inheriting from `AbstractStartOptionValueParser`
  
**If you are using the command-based approach**, make sure to register your custom value parser by calling before executing the `Run` method of your application:
```csharp
var instance = new CustomOptionValueParser();
StartOptionValueParserRegistry.Register(instance);
```

---

### Final notes and hints
1. Please note, that the help printer by default will display all available `StartOptions` **only** if the commandline arguments provided by the user only contained `HelpOptions` and nothing else. If there are `StartOptions` or `StartOptionGroups` contained in the command line arguments, the help page will only contain these options.
In order to change that, override the following method and provide the `PrintHelpPage` method with all available `StartOptions` and `StartOptionGroups`:
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

2. If you wish to use the same groupless `StartOption` with the **command-based approach** across multiple different constructors or classes, at first implement one command with a constructor parameter decorated with `GrouplessStartOptionAttribute` and another one (another constructor in the same or a different class) decorated with `GrouplessStartOptionReferenceAttribute`. Make sure that no groupless start options use duplicate names:
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

2. 1 You can also decorate your `CommandApplication` or any of your `IApplicationCommand` implementations with `GrouplessStartOptionAttribute` if you prefer your grouless StartOptions to be declared in a central location:
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

3. If you want to handle groupless StartOptions of your `CommandApplication` in a central location, you can attach handlers to your application (note: right now you can only attach one handler per groupless StartOption) by using these methods either in your application's constructor or from outside your application class:
```csharp
CommandApplication app = new DemoApplication();
//for groupless StartOptions of type Multiple
app.AddGlobalGrouplessStartOptionHandler<T>("numbers", (int[] _numbers) => this.Numbers = _numbers);
//for groupless StartOptions of type Single
app.AddGlobalGrouplessStartOptionHandler<T>("user", (string _username) => this.Username = _username);
//for groupless StartOptions of type Switch
app.AddGlobalGrouplessStartOptionHandler("verbose", () => this.Verbose = true);
```

4. **Caveat when using "/" as a prefix:** Please keep in mind that the prefix "/" for short or long StartOption names can conflict with certain CLI environments, such as git bash for Windows, which could interpret options starting with a "/" as an absolute path to a file or application instead of just a simple string that should be passed to your application.

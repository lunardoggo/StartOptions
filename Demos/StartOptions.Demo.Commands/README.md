## StartOptions.Demo.Commands
This example demonstrates how to utilize the command-based StartOption approach without dependency injection. Simply create some comand classes implementing [IApplicationCommand](https://github.com/lunardoggo/StartOptions/wiki/IApplicationCommand), an application class that inherits from [CommandApplication](https://github.com/lunardoggo/StartOptions/wiki/CommandApplication), override the abstract methods, register all available commands and call your application from the `Main` method.

This demo can be called from the command line like so:
```
StartOptions.Demo.Commands.exe /add --1 4 --2 65
```
```
StartOptions.Demo.Commands.exe /add --1 4 --2 -65
```
```
StartOptions.Demo.Commands.exe /read <path to your file here>
```

### Caveat when using "/" as a prefix
Please keep in mind that the prefix "/" for short or long StartOption names can conflict with certain CLI environments, such as git bash on Windows, which could interpret options starting with a "/" as an absolute path to a file or application instead of just a simple string that is passed to your application:
```
$ ./StartOptions.Demo.Commands.exe /add --1 4 --2 65
Unhandled exception. LunarDoggo.StartOptions.Exceptions.UnknownOptionNameException: Encountered unknown start options: C:/Program Files/Git/add, --1, --2
```
According to the error message, git bash for windows resolved the option "/add" to its root path "C:/Program Files/Git/" and tried to find an application called "add".
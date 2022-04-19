## StartOptions.Demo.Builders
This example demonstrates how to utilize the builder-based StartOption approach. Simply create a class that inherits from [AbstractApplication](https://github.com/lunardoggo/StartOptions/wiki/AbstractApplication), override the abstract methods and call your application from the `Main` method.

This demo can be called from the command line like so:
```
StartOptions.Demo.Builders.exe /add --1 4 --2 65
```
```
StartOptions.Demo.Builders.exe /add --1 4 --2 -65
```
```
StartOptions.Demo.Builders.exe /read <path to your file here>
```

### Caveat when using "/" as a prefix
Please keep in mind that the prefix "/" for short or long StartOption names can conflict with certain CLI environments, such as git bash on Windows, which could interpret options starting with a "/" as an absolute path to a file or application instead of just a simple string that is passed to your application:
```
$ ./StartOptions.Demo.Builders.exe /add --1 4 --2 65
Unhandled exception. LunarDoggo.StartOptions.Exceptions.UnknownOptionNameException: Encountered unknown start options: C:/Program Files/Git/add, --1, --2
```
According to the error message, git bash for windows resolved the option "/add" to its root path "C:/Program Files/Git/" and tried to find an application called "add".
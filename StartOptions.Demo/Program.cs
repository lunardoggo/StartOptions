using System;

namespace StartOptions.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DemoApplication application = new DemoApplication();
            application.Run(args);
            Console.WriteLine("Execution finished");
        }
    }
}

using System;

namespace StartOptions.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            DemoApplication application = new DemoApplication();
            application.AddGlobalGrouplessStartOptionHandler("verbose", () => Program.Verbose = true);
            application.Run(args);
            Console.WriteLine("Execution finished");
        }

        public static bool Verbose { get; set; }
    }
}

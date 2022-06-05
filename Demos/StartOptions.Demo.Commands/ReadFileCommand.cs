using LunarDoggo.StartOptions;
using System.IO;
using System;

namespace StartOptions.Demo
{
    public class ReadFileCommand : IApplicationCommand
    {
        private readonly string filePath;
        private readonly bool verbose;

        [StartOptionGroup("read", "r", Description = "Reads the specified file to the console")]
        public ReadFileCommand([StartOption("path", "p", Description = "Path to the file", IsMandatory = true, ValueType = StartOptionValueType.Single)]string path,
                               [GrouplessStartOption("verbose", "v", Description = "Enables verbose output")]bool verbose)
        {
            this.verbose = verbose;
            this.filePath = path;
        }

        public void Execute()
        {
            Console.WriteLine("Enable verbose output: " + this.verbose);
            if (File.Exists(this.filePath))
            {
                Console.WriteLine("Contents of file \"{0}\":\n", this.filePath);
                Console.WriteLine(File.ReadAllText(this.filePath));
            }
            else
            {
                Console.WriteLine("File \"{0}\" not found", this.filePath);
            }
        }
    }
}

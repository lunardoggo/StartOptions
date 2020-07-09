namespace LunarDoggo.StartOptions.Parsing
{
    internal class ParsedStartArgument
    {
        public ParsedStartArgument(string nameWithPrefix, string name, string value, bool isShortName)
        {
            this.NameWithPrefix = nameWithPrefix;
            this.IsShortName = isShortName;
            this.HasValue = value != null;
            this.Value = value;
            this.Name = name;
        }

        public string NameWithPrefix { get; }
        public bool IsShortName { get; }
        public bool HasValue { get; }
        public string Value { get; }
        public string Name { get; }
    }
}

namespace LunarDoggo.StartOptions.Building
{
    public struct StartOptionName
    {
        public StartOptionName(string prefix, string name)
        {
            this.Prefix = prefix;
            this.Name = name;
        }

        public string Prefix { get; }
        public string Name { get; }
    }
}

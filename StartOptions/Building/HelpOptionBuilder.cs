namespace LunarDoggo.StartOptions.Building
{
    public class HelpOptionBuilder
    {
        private readonly string name;

        private bool isShortName;

        public HelpOptionBuilder(string name)
        {
            this.name = name;
        }

        public HelpOptionBuilder SetIsShortName(bool isShortName = true)
        {
            this.isShortName = isShortName;
            return this;
        }

        public HelpOptionBuilder SetIsLongName(bool isLongName = true)
        {
            return this.SetIsShortName(!isLongName);
        }

        public HelpOption Build()
        {
            return new HelpOption(this.name, this.isShortName);
        }
    }
}

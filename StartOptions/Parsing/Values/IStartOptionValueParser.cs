namespace LunarDoggo.StartOptions.Parsing.Values
{
    public interface IStartOptionValueParser
    {
        object[] ParseMultiple(string[] values);
        object ParseSingle(string value);
    }
}

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public interface IStartOptionValueParser
    {
        object[] ParseValues(string[] values);
        object ParseValue(string value);
    }
}

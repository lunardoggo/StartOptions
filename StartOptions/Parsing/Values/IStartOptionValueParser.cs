namespace LunarDoggo.StartOptions.Parsing.Values
{
    public interface IStartOptionValueParser
    {
        /// <summary>
        /// Returns the objects parsed from the provided values
        /// </summary>
        object[] ParseValues(string[] values);
        /// <summary>
        /// Returns the object parsed from the provided value
        /// </summary>
        object ParseValue(string value);
    }
}

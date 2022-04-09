using System.Collections.Generic;
using System.Globalization;
using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public static class StartOptionValueParserRegistry
    {
        private static Dictionary<Type, IStartOptionValueParser> parsers = new Dictionary<Type, IStartOptionValueParser>();

        static StartOptionValueParserRegistry()
        {
            StartOptionValueParserRegistry.Register(new DoubleOptionValueParser(CultureInfo.CurrentCulture, NumberStyles.Number));
            StartOptionValueParserRegistry.Register(new FloatOptionValueParser(CultureInfo.CurrentCulture, NumberStyles.Number));
            StartOptionValueParserRegistry.Register(new Int16OptionValueParser());
            StartOptionValueParserRegistry.Register(new Int32OptionValueParser());
            StartOptionValueParserRegistry.Register(new Int64OptionValueParser());
            StartOptionValueParserRegistry.Register(new BoolOptionValueParser());
            StartOptionValueParserRegistry.Register(new ByteOptionValueParser());
        }

        public static void Register(IStartOptionValueParser parser)
        {
            if (parser != null)
            {
                StartOptionValueParserRegistry.parsers.Add(parser.GetType(), parser);
            }
        }

        public static void Deregister(IStartOptionValueParser parser)
        {
            if (parser != null && StartOptionValueParserRegistry.parsers.ContainsKey(parser.GetType()))
            {
                StartOptionValueParserRegistry.parsers.Remove(parser.GetType());
            }
        }

        public static void Deregister<T>()
        {
            if (StartOptionValueParserRegistry.parsers.ContainsKey(typeof(T)))
            {
                StartOptionValueParserRegistry.parsers.Remove(typeof(T));
            }
        }

        public static IStartOptionValueParser GetParser(Type key)
        {
            if (key != null && StartOptionValueParserRegistry.parsers.ContainsKey(key))
            {
                return StartOptionValueParserRegistry.parsers[key];
            }
            return null;
        }

        public static IStartOptionValueParser GetParser<T>()
        {
            return StartOptionValueParserRegistry.GetParser(typeof(T));
        }
    }
}

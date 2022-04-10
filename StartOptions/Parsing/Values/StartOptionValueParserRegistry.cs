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

        /// <summary>
        /// Registers the provided instance of <see cref="IStartOptionValueParser"/> for later use by instances of <see cref="StartOption"/>.
        /// Every single <see cref="Type"/> implementing <see cref="IStartOptionValueParser"/> can only be registered once
        /// </summary>
        public static void Register(IStartOptionValueParser parser)
        {
            if (parser != null)
            {
                StartOptionValueParserRegistry.parsers.Add(parser.GetType(), parser);
            }
        }

        /// <summary>
        /// Deregisters the provided <see cref="IStartOptionValueParser"/>, so it can't be used by instances od <see cref="StartOption"/> later
        /// </summary>
        public static void Deregister(IStartOptionValueParser parser)
        {
            if (parser != null && StartOptionValueParserRegistry.parsers.ContainsKey(parser.GetType()))
            {
                StartOptionValueParserRegistry.parsers.Remove(parser.GetType());
            }
        }

        /// <summary>
        /// Deregisters the provided <see cref="IStartOptionValueParser"/> of the provided <see cref="Type"/> <see cref="{T}"/>, so
        /// it can't be used by instances od <see cref="StartOption"/> later
        /// </summary>
        public static void Deregister<T>()
        {
            if (StartOptionValueParserRegistry.parsers.ContainsKey(typeof(T)))
            {
                StartOptionValueParserRegistry.parsers.Remove(typeof(T));
            }
        }

        /// <summary>
        /// Returns the registered instance <see cref="IStartOptionValueParser"/> of the provided <see cref="Type"/>
        /// </summary>
        public static IStartOptionValueParser GetParser(Type key)
        {
            if (key != null && StartOptionValueParserRegistry.parsers.ContainsKey(key))
            {
                return StartOptionValueParserRegistry.parsers[key];
            }
            return null;
        }

        /// <summary>
        /// Returns the registered instance <see cref="IStartOptionValueParser"/> <see cref="Type"/> <see cref="{T}"/>
        /// </summary>
        public static IStartOptionValueParser GetParser<T>()
        {
            return StartOptionValueParserRegistry.GetParser(typeof(T));
        }
    }
}

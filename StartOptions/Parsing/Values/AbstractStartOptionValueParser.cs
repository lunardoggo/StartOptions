using System.Linq;
using System;

namespace LunarDoggo.StartOptions.Parsing.Values
{
    public abstract class AbstractStartOptionValueParser : IStartOptionValueParser
    {
        private readonly Type parsedType;

        public virtual Type ParsedType { get { return this.parsedType; } }

        public AbstractStartOptionValueParser(Type parsedType)
        {
            this.parsedType = parsedType;
        }

        public object[] ParseValues(string[] values)
        {
            return values.Select(_value => this.ParseValue(_value)).ToArray();
        }

        public object ParseValue(string value)
        {
            try
            {
                return this.ParseSingleValue(value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Couldn't parse \"{value}\" into {this.parsedType.FullName}", ex);
            }
        }

        protected abstract object ParseSingleValue(string value);
    }
}

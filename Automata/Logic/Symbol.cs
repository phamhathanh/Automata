using System;

namespace Automata
{
    struct Symbol : IEquatable<Symbol>
    {
        private readonly char value;

        public Symbol(char c)
        {
            this.value = c;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator Symbol(char c)
        {
            return new Symbol(c);
        }

        public bool Equals(Symbol other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Symbol && this == (Symbol)obj;
        }

        public static bool operator==(Symbol symbol1, Symbol symbol2)
        {
            return symbol1.value == symbol2.value;
        }

        public static bool operator !=(Symbol symbol1, Symbol symbol2)
        {
            return symbol1.value != symbol2.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}

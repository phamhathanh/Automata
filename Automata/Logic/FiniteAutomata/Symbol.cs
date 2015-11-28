using System;

namespace Automata
{
    struct Symbol : IEquatable<Symbol>
    {
        private readonly string representation;

        public Symbol(char c)
        {
            this.representation = c.ToString();
        }

        public Symbol(string s)
        {
            this.representation = s;
        }

        public override string ToString()
        {
            return representation;
        }

        public char ToChar()
        {
            return representation[0];
        }

        public static implicit operator Symbol(char c)
        {
            return new Symbol(c);
        }

        public static implicit operator Symbol(string s)
        {
            return new Symbol(s);
        }

        public bool Equals(Symbol other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return obj is Symbol && this == (Symbol)obj;
        }

        public static bool operator ==(Symbol symbol1, Symbol symbol2)
        {
            return symbol1.representation == symbol2.representation;
        }

        public static bool operator !=(Symbol symbol1, Symbol symbol2)
        {
            return symbol1.representation != symbol2.representation;
        }

        public override int GetHashCode()
        {
            return representation.GetHashCode();
        }
    }
}
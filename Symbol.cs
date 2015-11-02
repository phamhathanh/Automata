using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Symbol
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

        public static implicit operator Symbol(char c)
        {
            return new Symbol(c);
        }

        public static implicit operator Symbol(string s)
        {
            return new Symbol(s);
        }
    }
}

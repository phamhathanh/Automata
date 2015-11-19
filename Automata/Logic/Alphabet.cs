using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Alphabet
    {
        private readonly Symbol[] symbols;
        public static readonly Symbol Epsilon = new Symbol('ε');
        public int Length
        {
            get
            {
                return symbols.Length;
            }
        }

        public Alphabet(Symbol[] symbols)
        {
            if (symbols.Contains(Epsilon))
                throw new ArgumentException("Alphabet cannot contain the epsilon symbol.");

            this.symbols = symbols.Distinct().ToArray();
        }

        public Symbol this[int index]
        {
            get
            {
                return symbols[index];
            }
        }

        public bool Contains(Symbol symbol)
        {
            foreach (Symbol s in symbols)
                if (symbol.Equals(s))
                    return true;

            if (symbol.Equals(Epsilon))
                return true;

            return false;
        }
    }
}

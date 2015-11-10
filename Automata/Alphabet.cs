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
        public static readonly Symbol epsilon = new Symbol("ε");

        public Alphabet(Symbol[] symbols)
        {
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

            if (symbol.Equals(epsilon))
                return true;

            return false;
        }
    }
}

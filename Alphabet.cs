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
        public static Symbol epsilon = new Symbol("ε");

        public Alphabet(char[] symbols)
        {
            this.symbols = (Symbol[])symbols.Clone();
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
                if (s.Equals(symbol))
                    return true;

            return false;
        }
    }
}

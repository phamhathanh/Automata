using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Alphabet
    {
        public static readonly Symbol Epsilon = new Symbol('ε');

        private readonly Symbol[] symbols;

        public Symbol this[int index]
        {
            get
            {
                return symbols[index];
            }
        }

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

            bool hasDuplicates = symbols.Distinct().Count() < symbols.Length;
            if (hasDuplicates)
                throw new ArgumentException("Alphabet cannot contain duplicates.");

            this.symbols = (Symbol[])symbols.Clone();
        }

        public bool Contains(Symbol symbol)
        {
            foreach (Symbol s in symbols)
                if (symbol.Equals(s))
                    return true;
            
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class GrammarAlphabet
    {
        public static readonly GrammarSymbol Epsilon = new GrammarSymbol("ε");

        private readonly GrammarSymbol[] symbols;

        public GrammarAlphabet(GrammarSymbol[] symbols)
        {
            if (symbols.Contains(Epsilon))
                throw new ArgumentException("Alphabet cannot contain the epsilon symbol.");

            bool hasDuplicates = symbols.Distinct().Count() < symbols.Length;
            if (hasDuplicates)
                throw new ArgumentException("Alphabet cannot contain duplicates.");

            this.symbols = (GrammarSymbol[])symbols.Clone();
        }
        
        public bool Contains(string symbol)
        {
            foreach (GrammarSymbol s in symbols)
                if (s.Matches(symbol))
                    return true;

            return false;
        }
    }
}

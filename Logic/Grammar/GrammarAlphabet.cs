using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class GrammarAlphabet
    {
        private readonly GrammarSymbol[] symbols;

        public GrammarAlphabet(GrammarSymbol[] symbols)
        {
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

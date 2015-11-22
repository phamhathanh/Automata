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
    }
}

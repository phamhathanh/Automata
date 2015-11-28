using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class Sentence : IEnumerable<GrammarSymbol>
    {
        private readonly GrammarSymbol[] symbols;

        public Sentence(GrammarSymbol[] symbols)
        {
            this.symbols = (GrammarSymbol[])symbols.Clone();
        }

        public IEnumerator<GrammarSymbol> GetEnumerator()
        {
            foreach (var symbol in symbols)
                yield return symbol;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

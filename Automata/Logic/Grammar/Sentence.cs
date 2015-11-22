using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class Sentence : IEnumerable<Symbol>
    {
        private Symbol[] symbols;

        public Sentence(Symbol[] symbols)
        {
            this.symbols = (Symbol[])symbols.Clone();
        }

        public IEnumerator<Symbol> GetEnumerator()
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

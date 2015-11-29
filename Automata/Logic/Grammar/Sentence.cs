using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Automata
{
    class Sentence : IEnumerable<GrammarSymbol>
    {
        public static Sentence Empty = new Sentence(new GrammarSymbol[] { });

        private readonly GrammarSymbol[] symbols;

        public int Length
        {
            get
            {
                return symbols.Length;
            }
        }

        public bool HasNonsolitaryTerminal
        {
            get
            {
                if (symbols.Length < 2)
                    return false;

                foreach (var symbol in symbols)
                    if (symbol.IsTerminal)
                        return true;

                return false;
            }
        }

        public bool HasMoreThanTwoNonterminals
        {
            get
            {
                return symbols.Count((s) => !s.IsTerminal) > 2;
                // can be more efficient
            }
        }

        public bool IsEmpty
        {
            get
            {
                return symbols.Length == 0;
            }
        }

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

        public override string ToString()
        {
            string output = "";
            foreach (var symbol in symbols)
                output += symbol.ToString();

            return output;
        }
    }
}

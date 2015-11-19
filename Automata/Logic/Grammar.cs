using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Grammar
    {
        private readonly Alphabet nonterminalSymbols, terminalSymbols;
        private readonly Symbol startingSymbol;
        private readonly Production[] rules;

        public Grammar(Symbol[] nonterminalSymbols, Symbol[] terminalSymbols,
                                                        Production[] rules, Symbol startingSymbol)
        {
            bool overlap = terminalSymbols.Intersect(nonterminalSymbols).Any();
            if (overlap)
                throw new ArgumentException("Terminal symbols and nonterminal symbols are not disjoint.");

            bool startSymbolIsNonterminal = nonterminalSymbols.Contains(startingSymbol);
            if (!startSymbolIsNonterminal)
                throw new ArgumentException("Start symbol must be a nonterminal symbol.");

            this.nonterminalSymbols = new Alphabet(nonterminalSymbols);
            this.terminalSymbols = new Alphabet(terminalSymbols);
            this.startingSymbol = startingSymbol;

            foreach (var rule in rules)
            {
                IsValid(rule);
                // do something
            }
        }

        protected virtual bool IsValid(Production rule)
        {
            return true;
        }


    }
}

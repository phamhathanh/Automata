using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class ContextFreeGrammar
    {
        private readonly Alphabet nonterminalSymbols, terminalSymbols, alphabet;
        private readonly Symbol startingSymbol;
        private readonly Production[] rules;

        public ContextFreeGrammar(Symbol[] nonterminalSymbols, Symbol[] terminalSymbols,
                                                        Production[] rules, Symbol startingSymbol)
        {
            bool overlap = terminalSymbols.Intersect(nonterminalSymbols).Any();
            if (overlap)
                throw new ArgumentException("Terminal symbols and nonterminal symbols are not disjoint.");

            bool startSymbolIsNonterminal = nonterminalSymbols.Contains(startingSymbol);
            if (!startSymbolIsNonterminal)
                throw new ArgumentException("Start symbol must be a nonterminal symbol.");

            foreach (var rule in rules)
                if (!IsValid(rule))
                    throw new ArgumentException("Production rule is invalid.");

            this.nonterminalSymbols = new Alphabet(nonterminalSymbols);
            this.terminalSymbols = new Alphabet(terminalSymbols);
            Symbol[] alphabet = nonterminalSymbols.Concat(terminalSymbols).ToArray();
            this.alphabet = new Alphabet(alphabet);

            this.startingSymbol = startingSymbol;
        }

        private bool IsValid(Production production)
        {
            Debug.Assert(nonterminalSymbols != null);
            Debug.Assert(terminalSymbols != null);
            Debug.Assert(alphabet != null);

            return nonterminalSymbols.Contains(production.Original)
                && alphabet.Contains(production.DirectDerivation);
        }

        public bool HasSentence(Sentence sentence)
        {
            throw new NotImplementedException();
        }
    }
}

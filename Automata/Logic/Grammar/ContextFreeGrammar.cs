using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Automata
{
    class ContextFreeGrammar
    {
        /*
        private readonly IEnumerable<NonterminalSymbol> nonterminals;
        private readonly IEnumerable<TerminalSymbol> terminals;
        private readonly IEnumerable<GrammarSymbol> alphabet;
        */
        private readonly Symbol starting;
        private readonly Production[] rules;

        public ContextFreeGrammar(string[] nonterminals, string[] terminals,
                                                            Production[] rules, string starting)
        {
            if (HasDuplicates(nonterminals))
                throw new ArgumentException("Nonterminal symbols cannot contain duplicates.");
            if (HasDuplicates(terminals))
                throw new ArgumentException("Terminal symbols cannot contain duplicates.");

            bool overlap = terminals.Intersect(nonterminals).Any();
            if (overlap)
                throw new ArgumentException("Terminal symbols and nonterminal symbols are not disjoint.");

            bool startSymbolIsNonterminal = nonterminals.Contains(starting);
            if (!startSymbolIsNonterminal)
                throw new ArgumentException("Start symbol must be a nonterminal symbol.");

            foreach (var rule in rules)
                if (!IsValid(rule))
                    throw new ArgumentException("Production rule is invalid.");

            /*
            int n = nonterminals.Length;
            this.nonterminals = new NonterminalSymbol[n];
            for (int i = 0; i < n; i++)
                this.nonterminals[i] = new NonterminalSymbol(nonterminals[i]);

            n = terminals.Length;
            this.terminals = new TerminalSymbol[n];
            for (int i = 0; i < n; i++)
                this.terminals[i] = new TerminalSymbol(terminals[i]);
            
            GrammarSymbol[] alphabet = ((GrammarSymbol)this.nonterminals).Concat(this.terminals).ToArray();
            */

            this.starting = starting;
        }

        private bool HasDuplicates(string[] symbols)
        {
            return symbols.Distinct().Count() < symbols.Length;
        }

        private bool IsValid(Production production)
        {
            throw new NotImplementedException();
            /*
            Debug.Assert(nonterminals != null);
            Debug.Assert(terminals != null);
            Debug.Assert(alphabet != null);

            return nonterminals.Contains(production.Original)
                && alphabet.Contains(production.DirectDerivation);
                */
        }

        public bool HasSentence(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        public ContextFreeGrammar GetChomskyNormalForm()
        {
            throw new NotImplementedException();
            EliminateStartingSymbolFromRHS();
            EliminateRulesWithNonsolitaryTerminals();
            EliminateRHSsWithMoreThanTwoNonterminals();
            EliminateEpsilonRules();
            EliminateUnitRules();
        }

        private void EliminateStartingSymbolFromRHS()
        {
            throw new NotImplementedException();
        }

        private void EliminateRulesWithNonsolitaryTerminals()
        {
            throw new NotImplementedException();
        }

        private void EliminateRHSsWithMoreThanTwoNonterminals()
        {
            throw new NotImplementedException();
        }

        private void EliminateEpsilonRules()
        {
            throw new NotImplementedException();
        }

        private void EliminateUnitRules()
        {
            throw new NotImplementedException();
        }
    }
}

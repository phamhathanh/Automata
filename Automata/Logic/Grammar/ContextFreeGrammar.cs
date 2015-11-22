using System;
using System.Diagnostics;
using System.Linq;

namespace Automata
{
    class ContextFreeGrammar
    {
        private readonly Alphabet nonterminals, terminals, alphabet;
        private readonly Symbol starting;
        private readonly Production[] rules;

        public ContextFreeGrammar(char[] nonterminals, char[] terminals,
                                                            Production[] rules, char starting)
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

            this.nonterminals = new Alphabet(nonterminals);
            this.terminals = new Alphabet(terminals);
            Symbol[] alphabet = nonterminals.Concat(terminals).ToArray();
            this.alphabet = new Alphabet(alphabet);

            this.starting = starting;
        }

        private bool HasDuplicates(char[] symbols)
        {
            return symbols.Distinct().Count() < symbols.Length;
        }

        private bool IsValid(Production production)
        {
            Debug.Assert(nonterminals != null);
            Debug.Assert(terminals != null);
            Debug.Assert(alphabet != null);

            return nonterminals.Contains(production.Original)
                && alphabet.Contains(production.DirectDerivation);
        }

        public bool HasSentence(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        public ContextFreeGrammar GetChomskyNormalForm()
        {
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Automata
{
    class ContextFreeGrammar
    {
        private readonly GrammarAlphabet nonterminals, terminals, alphabet;

        private readonly GrammarSymbol starting;
        private readonly Production[] rules;

        public ContextFreeGrammar(string[] nonterminals, string[] terminals,
                                                            Production[] rules, string starting)
        {
            if (HasDuplicates(nonterminals))
                throw new ArgumentException("Nonterminal symbols cannot contain duplicates.");
            if (HasDuplicates(terminals))
                throw new ArgumentException("Terminal symbols cannot contain duplicates.");
            if (HasDuplicates(rules))
                throw new ArgumentException("Terminal symbols cannot contain duplicates.");

            bool overlap = terminals.Intersect(nonterminals).Any();
            if (overlap)
                throw new ArgumentException("Terminal symbols and nonterminal symbols are not disjoint.");

            var nonterminalArray = SymbolArrayFromStringArray(nonterminals);
            this.nonterminals = new GrammarAlphabet(nonterminalArray);
            var terminalArray = SymbolArrayFromStringArray(terminals);
            this.terminals = new GrammarAlphabet(terminalArray);

            this.alphabet = new GrammarAlphabet((nonterminalArray).Concat(terminalArray).ToArray());

            int startingIndex = Array.IndexOf(nonterminals, starting);
            if (startingIndex < 0)
                throw new ArgumentException("Start symbol must be a nonterminal symbol.");

            this.starting = nonterminalArray[startingIndex];

            foreach (var rule in rules)
                if (!IsValid(rule))
                    throw new ArgumentException("Production rule is invalid.");

            // rules
        }

        private bool HasDuplicates(string[] symbols)
        {
            return symbols.Distinct().Count() < symbols.Length;
        }

        private bool HasDuplicates(Production[] rules)
        {
            int n = rules.Length;
            for (int i = 0; i < n - 1; i++)
                for (int j = i + 1; j < n; j++)
                    if (rules[i] == rules[j])
                        return true;

            return false;
        }

        private GrammarSymbol[] SymbolArrayFromStringArray(string[] symbols)
        {
            int n = symbols.Length;
            var output = new GrammarSymbol[n];
            for (int i = 0; i < n; i++)
                output[i] = new GrammarSymbol(symbols[i]);

            return output;
        }

        private bool IsValid(Production production)
        {
            Debug.Assert(nonterminals != null);
            Debug.Assert(terminals != null);
            Debug.Assert(alphabet != null);

            if (!nonterminals.Contains(production.Original))
                return false;

            foreach (var symbol in production.DirectDerivation)
                if (!alphabet.Contains(symbol))
                    return false;

            return true;
        }

        public bool HasSentence(Sentence sentence)
        {
            throw new NotImplementedException();
            // TODO: CYK here
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

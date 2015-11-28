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
                                                            ProductionInfo[] rules, string starting)
        {
            if (HasDuplicates(nonterminals))
                throw new ArgumentException("Nonterminal symbols cannot contain duplicates.");
            if (HasDuplicates(terminals))
                throw new ArgumentException("Terminal symbols cannot contain duplicates.");
            if (HasDuplicates(rules))
                throw new ArgumentException("Rules cannot contain duplicates.");

            bool overlap = terminals.Intersect(nonterminals).Any();
            if (overlap)
                throw new ArgumentException("Terminal symbols and nonterminal symbols are not disjoint.");

            var symbolByString = new Dictionary<string, GrammarSymbol>(nonterminals.Length + terminals.Length);

            var nonterminalArray = new GrammarSymbol[nonterminals.Length];
            for (int i = 0; i < nonterminals.Length; i++)
            {
                nonterminalArray[i] = new GrammarSymbol(nonterminals[i], false);
                symbolByString.Add(nonterminals[i], nonterminalArray[i]);
            }
            this.nonterminals = new GrammarAlphabet(nonterminalArray);

            var terminalArray = new GrammarSymbol[terminals.Length];
            for (int i = 0; i < nonterminals.Length; i++)
            {
                terminalArray[i] = new GrammarSymbol(terminals[i], false);
                symbolByString.Add(terminals[i], terminalArray[i]);
            }
            this.nonterminals = new GrammarAlphabet(nonterminalArray);

            var alphabetArray = (nonterminalArray).Concat(terminalArray).ToArray();
            this.alphabet = new GrammarAlphabet(alphabetArray);

            int startingIndex = Array.IndexOf(nonterminals, starting);
            if (startingIndex < 0)
                throw new ArgumentException("Start symbol must be a nonterminal symbol.");

            this.starting = nonterminalArray[startingIndex];

            var ruleList = new List<Production>(rules.Length);
            foreach (var rule in rules)
            {
                if (!IsValid(rule))
                    throw new ArgumentException("Production rule is invalid.");

                GrammarSymbol original = symbolByString[rule.Original];

                var sentenceSymbols = new List<GrammarSymbol>();
                foreach (string s in rule.DirectDerivative)
                    sentenceSymbols.Add(symbolByString[s]);
                Sentence sentence = new Sentence(sentenceSymbols.ToArray());

                ruleList.Add(new Production(original, sentence));
            }
            this.rules = ruleList.ToArray();
        }

        private bool HasDuplicates(string[] symbols)
        {
            return symbols.Distinct().Count() < symbols.Length;
        }

        private bool HasDuplicates(ProductionInfo[] rules)
        {
            int n = rules.Length;
            for (int i = 0; i < n - 1; i++)
                for (int j = i + 1; j < n; j++)
                    if (rules[i] == rules[j])
                        return true;

            return false;
        }

        private bool IsValid(ProductionInfo production)
        {
            Debug.Assert(nonterminals != null);
            Debug.Assert(terminals != null);
            Debug.Assert(alphabet != null);

            if (!nonterminals.Contains(production.Original))
                return false;

            foreach (var symbol in production.DirectDerivative)
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

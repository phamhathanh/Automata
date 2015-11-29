using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Automata
{
    class ContextFreeGrammar
    {
        private readonly GrammarSymbol[] nonterminals, terminals, alphabet;
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

            this.nonterminals = new GrammarSymbol[nonterminals.Length];
            for (int i = 0; i < nonterminals.Length; i++)
            {
                this.nonterminals[i] = new GrammarSymbol(nonterminals[i], false);
                symbolByString.Add(nonterminals[i], this.nonterminals[i]);
            }

            this.terminals = new GrammarSymbol[terminals.Length];
            for (int i = 0; i < terminals.Length; i++)
            {
                this.terminals[i] = new GrammarSymbol(terminals[i], true);
                symbolByString.Add(terminals[i], this.terminals[i]);
            }

            this.alphabet = (this.nonterminals).Concat(this.terminals).ToArray();


            int startingIndex = Array.IndexOf(nonterminals, starting);
            if (startingIndex < 0)
                throw new ArgumentException("Start symbol must be a nonterminal symbol.");

            this.starting = this.nonterminals[startingIndex];


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

        private ContextFreeGrammar(GrammarSymbol[] nonterminals, GrammarSymbol[] terminals,
                                                            Production[] rules, GrammarSymbol starting)
        {
            // doesnt check for exception, careful

            this.nonterminals = nonterminals;
            this.terminals = terminals;
            this.alphabet = nonterminals.Concat(terminals).ToArray();
            this.rules = rules;
            this.starting = starting;
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

            if (!AlphabetContains(nonterminals, production.Original))
                return false;

            foreach (var symbol in production.DirectDerivative)
                if (!AlphabetContains(alphabet, symbol))
                    return false;

            return true;
        }

        private bool AlphabetContains(GrammarSymbol[] array, string symbol)
        {
            foreach (GrammarSymbol s in array)
                if (s.Matches(symbol))
                    return true;

            return false;
        }

        public ContextFreeGrammar GetChomskyNormalForm()
        {
            var newNonterminals = new List<GrammarSymbol>(nonterminals);
            var newRules = new List<Production>(rules);
            var toBeAddedRules = new List<Production>();


            var newStarting = new GrammarSymbol("S0", false);
            newNonterminals.Add(newStarting);
            newRules.Add(new Production(newStarting, new Sentence(new[] { starting })));

            for (int i = 0; i < newRules.Count; i++)
            {
                var rule = newRules[i];

                var sentence = rule.DirectDerivative.ToArray();
                if (rule.DirectDerivative.HasNonsolitaryTerminal)
                {
                    var encountered = new List<GrammarSymbol>();
                    for (int j = 0; j < sentence.Length; j++)
                    {
                        var symbol = sentence[j];

                        if (symbol.IsTerminal)
                        {
                            string label = "N" + symbol.ToString();
                            GrammarSymbol newSymbol = encountered.FirstOrDefault((s) => s.Matches(label));
                            if (newSymbol == null)
                            {
                                newSymbol = new GrammarSymbol(label, false);
                                encountered.Add(newSymbol);
                                newNonterminals.Add(newSymbol);
                                toBeAddedRules.Add(new Production(newSymbol, new Sentence(new[] { symbol })));
                            }

                            sentence[j] = newSymbol;
                        }
                    }
                }

                newRules[i] = new Production(rule.Original, new Sentence(sentence));
            }
            newRules.AddRange(toBeAddedRules);
            toBeAddedRules = new List<Production>();


            for (int i = 0; i < newRules.Count; i++)
            {
                var rule = newRules[i];

                var sentence = rule.DirectDerivative;
                var newSentence = sentence.ToArray();
                if (sentence.HasMoreThanTwoNonterminals)
                {
                    GrammarSymbol nextSymbol = rule.Original;
                    int n = newSentence.Length;
                    for (int j = 0; j < n - 2; j++)
                    {
                        var currentSymbol = nextSymbol;
                        nextSymbol = new GrammarSymbol(rule.Original.ToString() + (i + 1).ToString(), false);
                        newNonterminals.Add(nextSymbol);
                        toBeAddedRules.Add(new Production(currentSymbol, new Sentence(new[] { newSentence[j], nextSymbol })));
                    }
                    newRules[i] = new Production(nextSymbol, new Sentence(new[] { newSentence[n - 2], newSentence[n - 1] }));
                    // order is somewhat scrambled
                }
            }
            newRules.AddRange(toBeAddedRules);
            toBeAddedRules = new List<Production>();


            bool noEpsilonLeft = false;
            while (!noEpsilonLeft)
            {
                noEpsilonLeft = true;
                foreach (var rule in newRules)
                    if (rule.DirectDerivative.IsEmpty && rule.Original != newStarting)
                    {

                        GrammarSymbol nullable = rule.Original;
                        bool isLastRule = newRules.Count((r) => r.Original == nullable) == 0;
                        if (!isLastRule)
                            for (int i = 0; i < newRules.Count(); i++)
                            {
                                var oldSymbol = newRules[i].Original;
                                var sentence = newRules[i].DirectDerivative.ToArray();
                                int n = sentence.Length;
                                Debug.Assert(n < 3);

                                if (n == 1 && sentence[0] == nullable)
                                    AddIfNotExist(ref newRules, new Production(oldSymbol, Sentence.Empty));
                                else if (n == 2)
                                {
                                    if (sentence[0] == sentence[1] && sentence[0] == nullable)
                                    {
                                        AddIfNotExist(ref newRules, new Production(oldSymbol, Sentence.Empty));
                                        AddIfNotExist(ref newRules, new Production(oldSymbol, new Sentence(new[] { sentence[0] })));
                                    }
                                    else if (sentence[0] == nullable)
                                        AddIfNotExist(ref newRules, new Production(oldSymbol, new Sentence(new[] { sentence[1] })));
                                    else if (sentence[1] == nullable)
                                        AddIfNotExist(ref newRules, new Production(oldSymbol, new Sentence(new[] { sentence[0] })));
                                }
                            }
                        else
                        {
                            for (int i = 0; i < newRules.Count(); i++)
                            {
                                var oldSymbol = newRules[i].Original;
                                var sentence = newRules[i].DirectDerivative.ToArray();
                                int n = sentence.Length;
                                Debug.Assert(n < 3);

                                if (n == 1 && sentence[0] == nullable)
                                    AddIfNotExist(ref newRules, new Production(oldSymbol, Sentence.Empty));
                                else if (n == 2)
                                {
                                    if (sentence[0] == sentence[1] && sentence[0] == nullable)
                                        AddIfNotExist(ref newRules, new Production(oldSymbol, Sentence.Empty));
                                    else if (sentence[0] == nullable)
                                        AddIfNotExist(ref newRules, new Production(oldSymbol, new Sentence(new[] { sentence[1] })));
                                    else
                                        AddIfNotExist(ref newRules, new Production(oldSymbol, new Sentence(new[] { sentence[0] })));
                                }
                            }
                            newNonterminals.Remove(nullable);
                        }
                        newRules.Remove(rule);
                        toBeAddedRules = new List<Production>();
                        noEpsilonLeft = false;
                        break;
                    }
            }

            bool noUnitRuleLeft = false;
            while (!noUnitRuleLeft)
            {
                noUnitRuleLeft = true;
                foreach (var rule in newRules)
                {
                    bool isUnitRule = rule.DirectDerivative.Length == 1 && !rule.DirectDerivative.First().IsTerminal;
                    if (isUnitRule)
                    {
                        newRules.Remove(rule);

                        var toBeReplaced = rule.DirectDerivative.First();
                        var replacement = rule.Original;
                        for (int i = 0; i < newRules.Count(); i++)
                        {
                            var oldSymbol = newRules[i].Original;
                            var sentence = newRules[i].DirectDerivative.ToArray();

                            if (oldSymbol == toBeReplaced)
                                toBeAddedRules.Add(new Production(replacement, new Sentence(sentence)));
                        }
                        newNonterminals.Remove(toBeReplaced);
                        newRules.AddRange(toBeAddedRules);
                        toBeAddedRules = new List<Production>();

                        noUnitRuleLeft = false;
                        break;
                    }
                }
            }

            return new ContextFreeGrammar(newNonterminals.ToArray(), this.terminals, newRules.ToArray(), newStarting);
        }

        private void AddIfNotExist(ref List<Production> newRules, Production toBeAdded)
        {
            foreach (Production rule in newRules)
                if (rule.Original == toBeAdded.Original
                 && rule.DirectDerivative.SequenceEqual(toBeAdded.DirectDerivative))
                    return;

            newRules.Add(toBeAdded);
        }

        public bool HasSentence(IEnumerable<string> sentence)
        {
            // TODO: force normal form somehow
            //       check alphabet

            if (!sentence.Any())
                return rules.Any((p) => p.Original == starting && p.DirectDerivative == Sentence.Empty);

            var input = sentence.ToArray();
            int n = input.Length;

            var table = new List<GrammarSymbol>[n][];
            for (int i = 0; i < n; i++)
            {
                table[i] = new List<GrammarSymbol>[i + 1];
                for (int j = 0; j < i; j++)
                    table[i][j] = new List<GrammarSymbol>();

                var query = from rule in rules
                            let derivative = rule.DirectDerivative
                            where derivative.Length == 1
                            && derivative.First().Matches(input[i])
                            select rule.Original;
                table[i][i] = query.ToList();
            }

            for (int k = 1; k < n; k++)
                for (int i = 0; i < n - k; i++)
                    for (int j = 0; j < k; j++)
                        foreach (Production rule in rules)
                        {
                            var derivative = rule.DirectDerivative;
                            if (derivative.Length != 2)
                                continue;

                            GrammarSymbol first = derivative.First(),
                                          second = derivative.ElementAt(1);
                            if (table[i + j][i].Contains(first) && table[i + k][i + j + 1].Contains(second))
                                table[i + k][i].Add(rule.Original);
                        }

            return table[n - 1][0].Contains(starting);
        }
    }
}

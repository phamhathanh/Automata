using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    /*
     *
     * All classes for CYK
     */
    class CYK
    {
        private CFG CNF; // No-empty-string CNF

        public CYK(CFG CNF)
        {
            this.CNF = CNF;
        }

        public bool isStringAcceptable(string inputString)
        {
            Symbol[] inputSymbols = new Symbol[inputString.Length];
            for (int i = 0; i < inputString.Length; i++)
            {
                inputSymbols[i] = inputString[i];
            }
            bool result = CYKSolve(inputSymbols);
            return result;
        }

        public bool isStringAcceptable(IEnumerable<Symbol> inputSymbols)
        {
            bool result = CYKSolve(inputSymbols);
            return result;
        }

        private bool CYKSolve(IEnumerable<Symbol> inputSymbols)
        {
            Symbol[] inputArray = inputSymbols.ToArray();

            int inputLength = inputArray.Count();
            if (inputLength == 0)
                return false; // Since No-empty-string CNF

            // declare matrix V
            IEnumerable<Symbol>[][] V = new IEnumerable<Symbol>[inputLength][];
            for (int i = 0; i < inputLength; i++)
            {
                V[i] = new IEnumerable<Symbol>[inputLength];
                for (int j = i; j < inputLength; j++)
                    V[i][j] = new Symbol[] { };
            }

            // calculate the main diagonal
            List<Tuple<Symbol, Symbol>> singleTerminalRules = new List<Tuple<Symbol, Symbol>>();
            foreach (Rule rule in CNF.rules)
            {
                if (rule.to.Count() == 1)
                {
                    singleTerminalRules.Add(new Tuple<Symbol, Symbol>(rule.from, rule.to.ToArray()[0]));
                }
            }
            for (int i = 0; i < inputLength; i++)
            {
                foreach (Tuple<Symbol, Symbol> rule in singleTerminalRules)
                {
                    if (rule.Item2 == inputArray[i])
                    {
                        var temp = V[i][i].ToList();
                        temp.Add(rule.Item1);
                        V[i][i] = temp.ToArray();
                        Console.WriteLine("V[" + i + "][" + i + "] += " + rule.Item1);
                    }
                }
            }

            // calculate V[i][j]
            // S -> AB ===> Tuple <S, A, B>
            List<Tuple<Symbol, Symbol[]>> ABTerminalRules = new List<Tuple<Symbol, Symbol[]>>();
            foreach (Rule rule in CNF.rules)
            {
                if (rule.to.Count() == 2)
                {
                    ABTerminalRules.Add(new Tuple<Symbol, Symbol[]>(rule.from, new Symbol[] { rule.to.ToArray()[0], rule.to.ToArray()[1] }));
                }
            }
            for (int k = 1; k < inputLength; k++)
            {
                // k: length - 1
                for (int i = 0; i < inputLength - k; i++)
                {
                    // i: offset, searching in a same-length set
                    for (int j = 0; j < k; j++)
                    {
                        Console.WriteLine("Xét V[" + i + "][" + (i + k) + "]");
                        // cut string into half, j: length of the 1st part
                        foreach (Tuple<Symbol, Symbol[]> ABTerminalRule in ABTerminalRules)
                        {
                            if (V[i][i + j].Contains(ABTerminalRule.Item2[0]) && V[i + j + 1][i + k].Contains(ABTerminalRule.Item2[1]))
                            {
                                var temp = V[i][i + k].ToList();
                                temp.Add(ABTerminalRule.Item1);
                                V[i][i + k] = temp.ToArray();
                                Console.WriteLine("==> V[" + i + "][" + (i + k) + "] += " + ABTerminalRule.Item1);
                            }
                        }
                        
                    }
                }
            }

            if (V[0][inputLength - 1].Contains(CNF.nonTerminals.ToArray()[0]))
                return true;

            return false;
        }
    }

    class CFG
    {
        // Context-Free Grammar
        public IEnumerable<Symbol> nonTerminals; // Start variable is the 0th element
        public IEnumerable<Symbol> terminals;
        public IEnumerable<Rule> rules;

        public CFG(IEnumerable<Symbol> nonTerminals, IEnumerable<Symbol> terminals, IEnumerable<Rule> rules)
        {
            this.nonTerminals = nonTerminals;
            this.terminals = terminals;
            this.rules = rules;
        }
    }

    class Rule
    {
        public Symbol from;
        public IEnumerable<Symbol> to;

        public Rule(Symbol from, IEnumerable<Symbol> to)
        {
            this.from = from;
            this.to = to;
        }

        public Rule(Symbol from, Symbol to)
        {
            this.from = from;
            this.to = new Symbol[] { to };
        }

        public Rule(Symbol from, char to)
        {
            this.from = from;
            this.to = new Symbol[] { to };
        }

        public Rule(Symbol from, string to)
        {
            // Only used for single-char symbol 'to' inputs
            this.from = from;

            if (to.Length > 0)
            {
                List<Symbol> toCharList = new List<Symbol>();
                foreach (char toChar in to)
                {
                    toCharList.Add(toChar);
                }
                this.to = toCharList.ToArray();
            }
        }
    }
}

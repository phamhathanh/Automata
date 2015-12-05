using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Automata.Logic;

namespace Automata.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //GrammarTest();
            //RegexTest();
            //PattersonTest();
            //CYKTest();
            NFAToDFA();

            Console.WriteLine();
            Console.WriteLine("All tests are passed.");
            Console.WriteLine("Press ENTER to continue.");
            Console.ReadLine();
        }

        private static void GrammarTest()
        {
            string[] nonterminals = new[] { "S" },
                    terminals = new[] { "0", "1" };
            string starting = "S";
            ProductionInfo[] rules = new[] { new ProductionInfo("S", new string[] {}),
                                        new ProductionInfo("S", new[] { "0", "S", "1"}),
                                        new ProductionInfo("S", new[] { "0", "S", "1"}) };

            try
            {
                var grammar = new ContextFreeGrammar(nonterminals, terminals, rules, starting);
            }
            catch (ArgumentException ex)
            {
                Debug.Assert(ex.Message == "Rules cannot contain duplicates.");
            }
        }

        private static void RegexTest()
        {
            string pattern = "(01|1)*(|0|000*)";
            // binary strings with no substring 001

            RegularLanguage language = new RegularLanguage(pattern);

            Debug.Assert(!language.Contains("001000"));
            Debug.Assert(language.Contains("010101"));
            Debug.Assert(language.Contains("01"));
            Debug.Assert(language.Contains("01\n"));
        }

        private static void PattersonTest()
        {
            string expression = "ab|aba|abba|babaa";
            Patterson patterson = new Patterson(expression);
            bool isUniquelyDecodable = patterson.IsUniquelyDecodable();
            Debug.Assert(isUniquelyDecodable);
        }

        private static void CYKTest()
        {
            string[] nonterminals = new[] { "S" },
                    terminals = new[] { "0", "1" };
            string starting = "S";
            ProductionInfo[] rules = new[] { new ProductionInfo("S", new[] {"0", "S", "1"}),
                                             new ProductionInfo("S", new string[] {}) };

            var contextFreeGrammar = new ContextFreeGrammar(nonterminals, terminals, rules, starting);
            var normalForm = contextFreeGrammar.GetChomskyNormalForm();

            string[] sentence1 = new[] { "0", "1", "1", "0", "0", "0" },
                     sentence2 = new[] { "0", "0", "0", "1", "1", "1" };
            Debug.Assert(!normalForm.HasSentence(sentence1));
            Debug.Assert(normalForm.HasSentence(sentence2));
        }

        private static void NFAToDFA()
        {
            var alphabet = new[] { '0', '1' };
            int statesCount = 4,
                initialIndex = 0;
            int[] acceptingIndexes = new[] { 2, 3 };
            var transitions = new[] { new Transition(0, 'ε', 2),
                                      new Transition(0, '0', 1),
                                      new Transition(1, '1', 1),
                                      new Transition(1, '1', 3),
                                      new Transition(2, 'ε', 1),
                                      new Transition(2, '0', 3),
                                      new Transition(3, '0', 2) };

            var nfa = new FiniteAutomaton(statesCount, alphabet, transitions, initialIndex, acceptingIndexes);
            var dfa = nfa.GetEquivalentDFA();

            statesCount = dfa.StatesCount;
            Debug.Assert(statesCount == 5);
        }
    }
}

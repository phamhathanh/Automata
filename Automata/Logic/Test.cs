using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Automata
{
    static class Test
    {
        public static void RunTest()
        {
            TestGrammar();
            TestRegex();
        }

        private static void TestGrammar()
        {
            string[] nonterminals = new[] { "S" },
                    terminals = new[] { "0", "1" };
            string starting = "S";
            Production[] rules = new[] { new Production("S", new[] { "ε" }),
                                        new Production("S", new[] { "0", "S", "1"}),
                                        new Production("S", new[] { "0", "S", "1"}) };

            Debug.Assert(rules.Distinct().Count() == 2);

            var grammar = new ContextFreeGrammar(nonterminals, terminals, rules, starting);
        }

        private static void TestRegex()
        {
            string pattern = "(01|1)*(|0|000*)";
            // binary strings with no substring 001

            RegularLanguage language = new RegularLanguage(pattern);
            Debug.Assert(!language.Contains("001000"));
            Debug.Assert(language.Contains("010101"));
            Debug.Assert(language.Contains("01"));
            Debug.Assert(language.Contains("01\n"));
        }
    }
}

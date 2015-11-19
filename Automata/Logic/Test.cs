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
            TestRegex();
        }

        private static void TestRegex()
        {
            string pattern = "(01|1)*(|0|000*)";
            // binary strings with no substring 001

            RegularLanguage language = new RegularLanguage(pattern);
            Debug.Assert(!language.Contains("001000"));
            Debug.Assert(language.Contains("010101"));
            Debug.Assert(language.Contains("01"));
        }
    }
}

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
            string pattern = "ab|aba|abba|babaa";
            // binary strings with no substring 001

            RegularLanguage language = new RegularLanguage(pattern);
            Automata.Logic.Patterson.Patterson patterson = new Logic.Patterson.Patterson(language);
            patterson.Check_Code();
            Debug.Assert(!language.Contains("001000"));
            Debug.Assert(language.Contains("010101"));
            Debug.Assert(language.Contains("01"));
            Debug.Assert(language.Contains("01\n"));
        }
    }
}

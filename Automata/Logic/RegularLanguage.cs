﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Automata
{
    class RegularLanguage
    {
        public static readonly RegularLanguage Empty = new RegularLanguage("");

        private readonly string pattern;
        private readonly Regex regex;

        public string Expression
        {
            get
            {
                return pattern;
            }
        }

        public RegularLanguage(string expression)
        {
            this.pattern = "^" + expression + "$";
            this.regex = new Regex(pattern);
        }

        public bool Contains(string input)
        {
            return regex.IsMatch(input);
        }
    }
}

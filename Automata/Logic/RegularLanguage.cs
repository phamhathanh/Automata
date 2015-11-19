using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Automata
{
    class RegularLanguage
    {
        private readonly Regex regex;

        public string Expression
        {
            get
            {
                return regex.ToString();
                // TODO: is shit
            }
        }

        public RegularLanguage(string expression)
        {
            string pattern = "";
            pattern += "^";

            pattern += "$";

            this.regex = new Regex(pattern);
        }
    }
}

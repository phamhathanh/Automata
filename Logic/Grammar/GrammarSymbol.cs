using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class GrammarSymbol
    {
        private readonly string representation;
        private readonly bool isTerminal;

        public bool IsTerminal
        {
            get
            {
                return isTerminal;
            }
        }

        public GrammarSymbol(string representation, bool isTerminal)
        {
            this.representation = representation;
            this.isTerminal = isTerminal;
        }

        public bool Matches(string s)
        {
            return s == representation;
        }

        public override string ToString()
        {
            return representation;
        }
    }
}

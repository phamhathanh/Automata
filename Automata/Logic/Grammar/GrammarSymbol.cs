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

        public bool IsTerminal { get; set; }

        public GrammarSymbol(string representation)
        {
            this.representation = representation;
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

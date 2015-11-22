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

        protected GrammarSymbol(string representation)
        {
            this.representation = representation;
        }
    }
}

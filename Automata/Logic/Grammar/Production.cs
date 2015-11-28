using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Production
    {
        private readonly GrammarSymbol original;
        private readonly Sentence directDerivative;

        public GrammarSymbol Original { get { return original; } }
        public Sentence DirectDerivative { get { return directDerivative; } }

        public Production(GrammarSymbol original, Sentence directDerivative)
        {
            this.original = original;
            this.directDerivative = directDerivative;
        }
    }
}

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

        private Sentence[] directDerivatives;

        public bool IsTerminal { get; set; }

        /* TODO: immutability, like the states
         *       no derivatives if terminal (may be only assert)
         */

        public GrammarSymbol(string representation)
        {
            this.representation = representation;
        }

        public IEnumerable<Sentence> GetDirectDerivatives()
        {
            throw new NotImplementedException();
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

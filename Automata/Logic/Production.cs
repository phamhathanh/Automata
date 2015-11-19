using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Production
    {
        private readonly string original, directDerivation;

        public Production(string original, string directDerivation)
        {
            this.original = original;
            this.directDerivation = directDerivation;
        }
    }
}

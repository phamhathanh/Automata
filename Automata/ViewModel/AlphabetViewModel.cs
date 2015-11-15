using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class AlphabetViewModel
    {
        private char[] symbols;

        public IEnumerable<char> Symbols
        {
            get
            {
                foreach (var symbol in symbols)
                    yield return symbol;
            }
        }

        public AlphabetViewModel(char[] symbols)
        {
            this.symbols = symbols.Distinct().ToArray();
        }
    }
}

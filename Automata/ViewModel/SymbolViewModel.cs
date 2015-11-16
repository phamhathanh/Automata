using System.ComponentModel;

namespace Automata
{
    class SymbolViewModel
    {
        private char symbol;

        public char Symbol
        {
            get
            {
                return symbol;
            }
        }

        public SymbolViewModel(char symbol)
        {
            this.symbol = symbol;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Alphabet
    {
        private readonly Symbol[] characters;
        public static Symbol epsilon = new Symbol("ε");

        public Alphabet(char[] characters)
        {
            this.characters = (Symbol[])characters.Clone();
        }

        public Symbol this[int index]
        {
            get
            {
                return characters[index];
            }
        }
    }
}

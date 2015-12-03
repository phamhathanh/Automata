using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Logic
{
    class Alphabet
    {
        public static readonly char Epsilon = 'ε';

        private readonly char[] characters;

        public Alphabet(char[] characters)
        {
            if (characters.Contains(Epsilon))
                throw new ArgumentException("Alphabet cannot contain the epsilon character.");

            bool hasDuplicates = characters.Distinct().Count() < characters.Length;
            if (hasDuplicates)
                throw new ArgumentException("Alphabet cannot contain duplicates.");

            this.characters = (char[])characters.Clone();
        }

        public bool Contains(char character)
        {
            return characters.Contains(character);
        }
    }
}

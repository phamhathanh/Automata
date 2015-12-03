using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Automata.Logic
{
    class Alphabet : IEnumerable<char>
    {
        public static readonly char Epsilon = 'ε';

        private readonly char[] characters;

        public Alphabet(char[] characters)
        {
            Debug.Assert(!characters.Contains(Epsilon));
            Debug.Assert(characters.Distinct().Count() == characters.Length);

            this.characters = (char[])characters.Clone();
        }

        public bool Contains(char character)
        {
            return characters.Contains(character);
        }

        public IEnumerator<char> GetEnumerator()
        {
            return ((IEnumerable<char>)characters).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

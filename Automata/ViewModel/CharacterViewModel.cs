using System.ComponentModel;

namespace Automata
{
    class CharacterViewModel
    {
        private char character;

        public char Character
        {
            get
            {
                return character;
            }
        }

        public CharacterViewModel(char character)
        {
            this.character = character;
        }
    }
}

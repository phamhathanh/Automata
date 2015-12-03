using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Logic
{
    internal class Transition
    {
        private readonly State currentState, nextState;
        private readonly char character;

        public State CurrentStateIndex
        {
            get
            {
                return currentState;
            }
        }

        public State NextStateIndex
        {
            get
            {
                return nextState;
            }
        }

        public char Symbol
        {
            get
            {
                return character;
            }
        }

        public Transition(State currentState, char character, State nextState)
        {
            this.currentState = currentState;
            this.character = character;
            this.nextState = nextState;
        }
    }
}

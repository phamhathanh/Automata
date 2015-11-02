using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    abstract class FiniteAutomaton
    {
        protected readonly Alphabet alphabet;
        protected readonly State[] states;
        protected readonly int initialStateIndex;
        protected readonly int[] finalStateIndexes;
        
        protected abstract State Transition(State current, Symbol symbol);
        public abstract bool AcceptString(string input);

        protected FiniteAutomaton(State[] states, int initialStateIndex, int[] finalStateIndexes)
        {
            this.states = (State[])states.Clone();

            if (initialStateIndex < 0 || initialStateIndex >= states.Length)
                throw new ArgumentOutOfRangeException("State index is out of range.");
            this.initialStateIndex = initialStateIndex;

            this.finalStateIndexes = new int[finalStateIndexes.Length];
            for (int i = 0; i < finalStateIndexes.Length; i++)
            {
                if (finalStateIndexes[i] < 0 || finalStateIndexes[i] > states.Length)
                    throw new ArgumentOutOfRangeException("State index is out of range.");
                this.finalStateIndexes[i] = finalStateIndexes[i];
            }
        }
    }
}

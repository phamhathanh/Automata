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
        protected readonly State initialState;
        protected readonly int[] acceptingStateIndexes;
        
        protected abstract State Transition(State current, Symbol symbol);
        public abstract bool AcceptString(string input);

        protected FiniteAutomaton(int statesCount, Alphabet alphabet, Transition[] transitions,
                                                    int initialStateIndex, int[] acceptingStateIndexes)
        {
            this.alphabet = alphabet;
            this.states = new State[statesCount];
            
            foreach (Transition transition in transitions)
            {
                CheckIfIndexIsValid(transition.CurrentStateIndex);
            }

            CheckIfIndexIsValid(initialStateIndex);
            this.initialState = states[initialStateIndex];

            this.acceptingStateIndexes = new int[acceptingStateIndexes.Length];
            for (int i = 0; i < acceptingStateIndexes.Length; i++)
            {
                CheckIfIndexIsValid(acceptingStateIndexes[i]);
                this.acceptingStateIndexes[i] = acceptingStateIndexes[i];
            }
        }

        private void CheckIfIndexIsValid(int index)
        {
            if (states == null)
                throw new InvalidOperationException("States collection is not yet initialized.");

            if (index < 0 || index >= states.Length)
                throw new ArgumentOutOfRangeException("State index is out of range.");
        }

        private void CheckIfSymbolIsValid(Symbol symbol)
        {

        }
    }
}

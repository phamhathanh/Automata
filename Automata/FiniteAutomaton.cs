using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    partial class FiniteAutomaton
    {
        protected readonly Alphabet alphabet;
        protected readonly State initialState;

        public FiniteAutomaton(int statesCount, Alphabet alphabet, Transition[] transitions,
                                                    int initialStateIndex, int[] acceptingStateIndexes)
        {
            this.alphabet = alphabet;
            State[] states = new State[statesCount];
            for (int i = 0; i < statesCount; i++)
                states[i] = new State();
            this.initialState = GetState(states, initialStateIndex);

            foreach (Transition transition in transitions)
                AddTransition(states, transition);
            
            for (int i = 0; i < acceptingStateIndexes.Length; i++)
            {
                State state = GetState(states, acceptingStateIndexes[i]);
                state.IsAccepting = true;
            }

            foreach (State state in states)
                state.WrapUp();
        }

        private void AddTransition(State[] states, Transition transition)
        {
            State currentState = GetState(states, transition.CurrentStateIndex),
                    nextState = GetState(states, transition.NextStateIndex);

            if (!alphabet.Contains(transition.Symbol))
                throw new ArgumentException("Symbol is not in the alphabet.");

            currentState.AddTransition(transition.Symbol, nextState);
        }

        private State GetState(State[] states, int stateIndex)
        {
            if (stateIndex < 0 || stateIndex >= states.Length)
                throw new ArgumentOutOfRangeException("State index is out of range.");

            return states[stateIndex];
        }

        public virtual bool AcceptString(string input)
        {
            IEnumerable<State> currentStates = new State[] { initialState };
            foreach (Symbol symbol in input)
                currentStates = NextStates(currentStates, symbol);

            IEnumerable<State> finalStates = EpsilonClosure(currentStates);
            return HasAcceptingState(finalStates);
        }

        private IEnumerable<State> NextStates(IEnumerable<State> states, Symbol symbol)
        {
            if (!alphabet.Contains(symbol))
                throw new ArgumentException("Symbol is not in the alphabet.");

            foreach (State state in states)
            {
                IEnumerable<State> nextStates = state.GetNextStates(symbol);
                foreach (State nextState in EpsilonClosure(nextStates))
                    yield return nextState;
            }
        }

        private IEnumerable<State> EpsilonClosure(IEnumerable<State> states)
        {
            Queue<State> consideredStates = new Queue<State>(states);
            List<State> closure = new List<State>(consideredStates.Count);

            while (consideredStates.Count != 0)
            {
                State state = consideredStates.Dequeue();
                closure.Add(state);

                IEnumerable<State> epsilonStates = state.GetNextStates(Alphabet.epsilon);
                foreach (State epsilonState in epsilonStates)
                    if (!closure.Contains(epsilonState))
                        consideredStates.Enqueue(epsilonState);
            }

            foreach (State state in closure)
                yield return state;

            // TODO: lazy iteration?
        }

        private bool HasAcceptingState(IEnumerable<State> states)
        {
            foreach (State state in states)
                if (state.IsAccepting)
                    return true;

            return false;
        }
    }
}

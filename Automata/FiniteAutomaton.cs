using System;
using System.Collections.Generic;

namespace Automata
{
    partial class FiniteAutomaton : IAutomaton
    {
        private readonly Alphabet alphabet;
        private readonly State[] states;
        private readonly State initialState;

        public FiniteAutomaton(int statesCount, Alphabet alphabet, Transition[] transitionInfos,
                                                    int initialStateIndex, int[] acceptingStateIndexes)
        {
            this.alphabet = alphabet;

            this.states = new State[statesCount];
            for (int i = 0; i < statesCount; i++)
                states[i] = new State();

            this.initialState = GetState(initialStateIndex);

            int transitionsCount = transitionInfos.Length;
            foreach (var info in transitionInfos)
                AddTransition(info);
            
            for (int i = 0; i < acceptingStateIndexes.Length; i++)
            {
                State state = GetState(acceptingStateIndexes[i]);
                state.IsAccepting = true;
            }

            foreach (State state in states)
                state.WrapUp();
        }

        private void AddTransition(Transition info)
        {
            if (!alphabet.Contains(info.Symbol))
                throw new ArgumentException("Symbol is not in the alphabet.");

            State currentState = GetState(info.CurrentStateIndex),
                    nextState = GetState(info.NextStateIndex);
            Symbol symbol = info.Symbol;

            currentState.AddTransition(symbol, nextState);
        }

        private State GetState(int stateIndex)
        {
            if (stateIndex < 0 || stateIndex >= states.Length)
                throw new ArgumentOutOfRangeException("State index is out of range.");

            return states[stateIndex];
        }

        public bool AcceptString(string input)
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
            Queue<State> toBeConsideredStates = new Queue<State>(states);
            List<State> closure = new List<State>(toBeConsideredStates.Count);

            while (toBeConsideredStates.Count != 0)
            {
                State state = toBeConsideredStates.Dequeue();
                closure.Add(state);
                yield return state;

                IEnumerable<State> epsilonStates = state.GetNextStates(Alphabet.Epsilon);
                foreach (State epsilonState in epsilonStates)
                {
                    bool stateIsAlreadyInClosure = closure.Contains(epsilonState),
                         stateIsBeingConsidered = toBeConsideredStates.Contains(epsilonState);
                    if (!stateIsAlreadyInClosure && !stateIsBeingConsidered)
                        toBeConsideredStates.Enqueue(epsilonState);
                }
            }
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

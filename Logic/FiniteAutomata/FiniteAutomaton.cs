using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Automata.Logic
{
    partial class FiniteAutomaton
    {
        private readonly Alphabet alphabet;
        private readonly State[] states;
        private readonly int initialIndex;
        private readonly int[] acceptingIndexes;
        private readonly TransitionTable table;

        public IEnumerable<char> Characters
        {
            get
            {
                foreach (char character in alphabet)
                    yield return character;
            }
        }
        public int StatesCount
        {
            get
            {
                return states.Length;
            }
        }
        public int InitialIndex
        {
            get
            {
                return initialIndex;
            }
        }
        public IEnumerable<int> AcceptingIndexes
        {
            get
            {
                foreach (int index in acceptingIndexes)
                    yield return index;
            }
        }

        //TODO: internal exposure?

        public FiniteAutomaton(int statesCount, char[] alphabet, Transition[] transitions,
                                                    int initialIndex, int[] acceptingIndexes)
        {
            ValidateAlphabet(alphabet);
            this.alphabet = new Alphabet(alphabet);

            this.states = new State[statesCount];
            for (int i = 0; i < statesCount; i++)
                this.states[i] = new State();

            if (ArrayHasDuplicates(acceptingIndexes))
                throw new ArgumentException("Accepting indexes cannot contain duplicates.");
            this.acceptingIndexes = (int[])acceptingIndexes.Clone();

            if (initialIndex < 0 || initialIndex >= states.Length)
                throw new ArgumentOutOfRangeException("Initial state index is out of range.");
            this.initialIndex = initialIndex;

            if (ArrayHasDuplicates(transitions))
                throw new ArgumentException("Transitions cannot contain duplicates.");
            this.table = new TransitionTable();

            foreach (var info in transitions)
                AddTransition(info);
        }

        private void ValidateAlphabet(char[] alphabet)
        {
            if (ArrayHasDuplicates(alphabet))
                throw new ArgumentException("Alphabet cannot contain duplicates.");
            if (alphabet.Contains(Alphabet.Epsilon))
                throw new ArgumentException("Alphabet cannot contain the epsilon character.");
        }

        private bool ArrayHasDuplicates<T>(T[] array)
        {
            return array.Distinct().Count() < array.Length;
        }

        private void AddTransition(Transition transition)
        {
            char character = transition.Character;
            bool isValid = character == Alphabet.Epsilon || alphabet.Contains(character);
            if (isValid)
                throw new ArgumentException("Character is invalid.");

            int currentIndex = transition.CurrentStateIndex,
                nextIndex = transition.NextStateIndex;
            State current = StateFromIndex(currentIndex),
                    next = StateFromIndex(nextIndex);

            Debug.Assert(table != null);
            table.Add(current, character, next);
        }

        private State StateFromIndex(int stateIndex)
        {
            Debug.Assert(states != null);

            if (stateIndex < 0 || stateIndex >= states.Length)
                throw new ArgumentOutOfRangeException("State index is out of range.");

            return states[stateIndex];
        }

        public bool AcceptString(string input)
        {
            var initial = StateFromIndex(initialIndex);
            var currents = table.GetInitialEpsilonClosure(initial);
            foreach (char character in input)
            {
                if (!alphabet.Contains(character))
                    throw new ArgumentException("Symbol is not in the alphabet.");
                currents = table.GetNextStates(currents, character);
            }

            return HasAcceptingState(currents);
        }

        private bool HasAcceptingState(IEnumerable<State> currents)
        {
            foreach (int index in acceptingIndexes)
            {
                State accepting = this.states[index];
                if (currents.Contains(accepting))
                    return true;
            }

            return false;
        }
    }
}

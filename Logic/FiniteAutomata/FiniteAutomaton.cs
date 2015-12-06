using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Automata.Logic
{
    public class FiniteAutomaton
    {
        private readonly Alphabet alphabet;
        private readonly State[] states, acceptings;
        private readonly State initial;
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
                return IndexOfState(initial);
            }
        }
        public IEnumerable<int> AcceptingIndexes
        {
            get
            {
                foreach (var state in acceptings)
                    yield return IndexOfState(state);
            }
        }
        public IEnumerable<Transition> Transitions
        {
            get
            {
                var transitions = table.GetAllTransitions();
                foreach (var transition in transitions)
                {
                    char character = transition.Item2;
                    State current = transition.Item1,
                          next = transition.Item3;
                    int currentIndex = IndexOfState(current),
                        nextIndex = IndexOfState(next);

                    yield return new Transition(currentIndex, character, nextIndex);
                }
            }
        }

        internal IEnumerable<State> States
        {
            get
            {
                foreach (var state in states)
                    yield return state;
            }
        }
        internal IEnumerable<State> AcceptingStates
        {
            get
            {
                foreach (var state in acceptings)
                    yield return state;
            }
        }
        internal State InitialState
        {
            get
            {
                return initial;
            }
        }
        internal TransitionTable Table
        {
            get
            {
                return table;
            }
        }

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
            int acceptingsCount = acceptingIndexes.Length;
            this.acceptings = new State[acceptingsCount];
            for (int i = 0; i < acceptingsCount; i++)
                this.acceptings[i] = StateFromIndex(acceptingIndexes[i]);

            this.initial = StateFromIndex(initialIndex);

            if (ArrayHasDuplicates(transitions))
                throw new ArgumentException("Transitions cannot contain duplicates.");
            this.table = new TransitionTable();

            foreach (var info in transitions)
                AddTransition(info);
            this.table.WrapUp();
        }

        internal FiniteAutomaton(IEnumerable<State> states, char[] alphabet, State initial,
                                IEnumerable<State> acceptings, TransitionTable table)
        {
            this.states = states.ToArray();

            ValidateAlphabet(alphabet);
            this.alphabet = new Alphabet(alphabet);

            if (!states.Contains(initial))
                throw new ArgumentException("Invalid initial state.");
            this.initial = initial;

            var acceptingList = new List<State>();
            foreach (var state in acceptings)
            {
                if (!states.Contains(state))
                    throw new ArgumentException("Invalid accepting state.");
                acceptingList.Add(state);
            }
            this.acceptings = acceptingList.ToArray();

            var tableStates = table.GetAllStates();
            foreach (var state in tableStates)
                if (!states.Contains(state))
                    throw new ArgumentException("Invalid table.");
            var tableChars = table.GetAllCharactersIncludingEpsilon();
            foreach (var character in tableChars)
                ValidateChar(character);
            this.table = table;
            this.table.WrapUp();
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
            ValidateChar(character);

            int currentIndex = transition.CurrentStateIndex,
                nextIndex = transition.NextStateIndex;
            State current = StateFromIndex(currentIndex),
                    next = StateFromIndex(nextIndex);

            Debug.Assert(table != null);
            table.Add(current, character, next);
        }

        private void ValidateChar(char character)
        {
            Debug.Assert(alphabet != null);

            bool isValid = character == Alphabet.Epsilon || alphabet.Contains(character);
            if (!isValid)
                throw new ArgumentException("Character is invalid.");
        }

        private State StateFromIndex(int stateIndex)
        {
            Debug.Assert(states != null);

            if (stateIndex < 0 || stateIndex >= states.Length)
                throw new ArgumentOutOfRangeException("State index is out of range.");

            return states[stateIndex];
        }

        private int IndexOfState(State state)
        {
            Debug.Assert(states != null);
            int index = Array.IndexOf(states, state);
            Debug.Assert(index > -1);

            return index;
        }

        public bool AcceptString(string input)
        {
            var currents = table.GetInitialEpsilonClosure(initial);
            foreach (char character in input)
            {
                if (!alphabet.Contains(character))
                    throw new ArgumentException("Character is not in the alphabet.");
                currents = table.GetNextStates(currents, character);
            }

            return HasAcceptingState(currents);
        }

        private bool HasAcceptingState(IEnumerable<State> currents)
        {
            foreach (var state in currents)
            {
                if (acceptings.Contains(state))
                    return true;
            }

            return false;
        }

        public FiniteAutomaton ToDFA()
        {
            // Algorithm: Powerset Construction

            bool isAlreadyDeterministic = IsDeterministic();
            if (isAlreadyDeterministic)
                return this;


            List<State> dfaStates = new List<State>(),
                    dfaAcceptings = new List<State>();

            var nfaStatesComparer = new OrderIndependentStatesComparer();
            var nfaToDFA = new Dictionary<IEnumerable<State>, State>(nfaStatesComparer);
            var dfaToNFA = new Dictionary<State, IEnumerable<State>>();

            State dfaInitial = new State();
            var nfaInitialClosure = table.GetInitialEpsilonClosure(initial);
            if (HasAcceptingState(nfaInitialClosure))
                dfaAcceptings.Add(dfaInitial);
            dfaStates.Add(dfaInitial);
            nfaToDFA.Add(nfaInitialClosure, dfaInitial);
            dfaToNFA.Add(dfaInitial, nfaInitialClosure);

            var dfaTable = new TransitionTable();
            var considerations = new Queue<State>(dfaStates);
            while (considerations.Count > 0)
            {
                var dfaState = considerations.Dequeue();
                IEnumerable<State> correspondingNFAStates;      // It is OK for this to be empty.
                bool success = dfaToNFA.TryGetValue(dfaState, out correspondingNFAStates);
                Debug.Assert(success);

                foreach (char character in alphabet)
                {
                    var nextNFAStates = table.GetNextStates(correspondingNFAStates, character);
                    State nextDFAState;
                    success = nfaToDFA.TryGetValue(nextNFAStates, out nextDFAState);
                    if (!success)
                    {
                        nextDFAState = new State();
                        if (HasAcceptingState(nextNFAStates))
                            dfaAcceptings.Add(nextDFAState);
                        dfaStates.Add(nextDFAState);
                        nfaToDFA.Add(nextNFAStates, nextDFAState);
                        dfaToNFA.Add(nextDFAState, nextNFAStates);
                        considerations.Enqueue(nextDFAState);
                    }
                    else
                        Debug.Assert(dfaStates.Contains(nextDFAState));

                    dfaTable.Add(dfaState, character, nextDFAState);
                }
            }

            return new FiniteAutomaton(dfaStates, alphabet.ToArray(), dfaInitial, dfaAcceptings, dfaTable);
        }

        private bool IsDeterministic()
        {
            foreach (var state in states)
            {
                foreach (char character in alphabet)
                {
                    var nexts = table.GetNextStates(state, character);
                    if (nexts.Count() != 1)
                        return false;
                }
                var epsilonMove = table.GetNextStates(state, Alphabet.Epsilon);
                if (epsilonMove.Count() > 0)
                    return false;
            }

            return true;
        }

        private class OrderIndependentStatesComparer : EqualityComparer<IEnumerable<State>>
        {
            public override bool Equals(IEnumerable<State> states1, IEnumerable<State> states2)
            {
                return GetHashCode(states1) == GetHashCode(states2);
            }

            public override int GetHashCode(IEnumerable<State> states)
            {
                // hashcode is order-independent

                var hashes = new List<int>();
                foreach (var state in states)
                    hashes.Add(state.GetHashCode());
                hashes.Sort();

                int output = 0;
                foreach (int hash in hashes)
                    unchecked
                    {
                        output *= 251;
                        output += hash;
                    }

                return output;
            }
        }

        public FiniteAutomaton ToMinimizedDFA()
        {
            var dfa = this.ToDFA();
            return dfa.Minimize();
        }

        private FiniteAutomaton Minimize()
        {
            // Algorithm: Table-filling
            // must operate on a DFA, or else


            var considerations = new Queue<StatePair>();
            var allPairs = new List<StatePair>();

            var reachableStates = GetReachableStates().ToArray();
#if DEBUG
            bool duplicatedStates = reachableStates.Distinct().Count() > reachableStates.Length;
            Debug.Assert(!duplicatedStates);
#endif

            foreach (var pair in GetAllPairs(reachableStates))
            {
                allPairs.Add(pair);
                bool canBeMarked = CanBeConcludedDistinguishable(pair);
                if (canBeMarked)
                    pair.Mark();
                else
                    considerations.Enqueue(pair);
            }

            while (considerations.Count > 0)
            {
                var thisPair = considerations.Dequeue();
                foreach (char character in alphabet)
                {
                    var nextPair = GetNextPair(thisPair, character);
                    if (nextPair.State1 == nextPair.State2)
                        continue;

                    var matches = from pair in allPairs
                                  where pair == nextPair
                                  select pair;
                    Debug.Assert(matches.Count() == 1);
                    var match = matches.First();

                    if (match.IsMarked)
                    {
                        thisPair.Mark();
                        break;
                    }
                    else if (considerations.Contains(match))
                        match.AddEquivalentPair(thisPair);
                }
            }

            var minimizedToDFA = new Dictionary<State, List<State>>();
            var dfaToMinimized = new Dictionary<State, State>();
            foreach (var dfaState in reachableStates)
            {
                var equivalentClass = new List<State>();
                equivalentClass.Add(dfaState);
                var minimizedState = new State();
                minimizedToDFA.Add(minimizedState, equivalentClass);
                dfaToMinimized.Add(dfaState, minimizedState);
            }
            foreach (var pair in allPairs)
            {
                if (!pair.IsMarked)
                {
                    State minimized1, minimized2;
                    bool success1 = dfaToMinimized.TryGetValue(pair.State1, out minimized1),
                         success2 = dfaToMinimized.TryGetValue(pair.State2, out minimized2);

                    Debug.Assert(success1);
                    Debug.Assert(success2);

                    List<State> class1, class2;
                    success1 = minimizedToDFA.TryGetValue(minimized1, out class1);
                    success2 = minimizedToDFA.TryGetValue(minimized2, out class2);

                    Debug.Assert(success1);
                    Debug.Assert(success2);

                    class1.AddRange(class2);
                    minimizedToDFA.Remove(minimized2);
                    dfaToMinimized[pair.State2] = minimized1;
                }
            }

            var newTable = new TransitionTable();
            var transitions = table.GetAllTransitions();
            foreach (var transition in transitions)
            {
                char character = transition.Item2;
                State dfaCurrent = transition.Item1,
                        dfaNext = transition.Item3;

                bool isIsolated = !reachableStates.Contains(dfaCurrent);
                if (isIsolated)
                    continue;

                State minimizedCurrent = dfaToMinimized[dfaCurrent],
                    minimizedNext = dfaToMinimized[dfaNext];

                bool merged = dfaCurrent != dfaNext && minimizedCurrent == minimizedNext;
                if (merged)
                    continue;

                var nextStates = newTable.GetNextStates(minimizedCurrent, character);
                bool duplicate = nextStates.Contains(minimizedNext);
                if (!duplicate)
                    newTable.Add(minimizedCurrent, character, minimizedNext);
            }

            var newStates = from equivalence in minimizedToDFA
                            select equivalence.Key;
            var newInitial = dfaToMinimized[initial];
            var newAccepting = from equivalence in minimizedToDFA
                               where acceptings.Contains(equivalence.Value.First())
                               select equivalence.Key;

            return new FiniteAutomaton(newStates.ToArray(), alphabet.ToArray(), newInitial, newAccepting, newTable);
        }

        private IEnumerable<State> GetReachableStates()
        {
            var yielded = new List<State>();
            var considerations = new Queue<State>();

            yield return initial;
            yielded.Add(initial);
            considerations.Enqueue(initial);

            while (considerations.Count != 0)
            {
                var state = considerations.Dequeue();
                foreach (char character in alphabet)
                {
                    var reachableStates = table.GetNextStates(state, character);
                    foreach (State reachable in reachableStates)
                    {
                        bool alreadyConsidered = yielded.Contains(reachable);
                        if (!alreadyConsidered)
                        {
                            yield return reachable;
                            yielded.Add(reachable);
                            considerations.Enqueue(reachable);
                        }
                    }
                }
            }
        }

        private IEnumerable<StatePair> GetAllPairs(IEnumerable<State> states)
        {
            var stateArray = states.ToArray();
            int statesCount = stateArray.Length;
            for (int i = 0; i < statesCount - 1; i++)
                for (int j = statesCount - 1; j > i; j--)
                {
                    var pair = new StatePair(stateArray[i], stateArray[j]);
                    yield return pair;
                }
        }

        private bool CanBeConcludedDistinguishable(StatePair pair)
        {
            var state1 = pair.State1;
            var state2 = pair.State2;
            bool isAccepting1 = acceptings.Contains(state1),
                 isAccepting2 = acceptings.Contains(state2);

            return isAccepting1 != isAccepting2;
        }

        private StatePair GetNextPair(StatePair current, char character)
        {
            State state1 = current.State1,
                  state2 = current.State2;

            var nextStates1 = table.GetNextStates(state1, character);
            var nextStates2 = table.GetNextStates(state2, character);

            Debug.Assert(nextStates1.Count() == 1);
            Debug.Assert(nextStates2.Count() == 1);

            State next1 = nextStates1.First(),
                  next2 = nextStates2.First();

            return new StatePair(next1, next2);
        }

        private class StatePair : IEquatable<StatePair>
        {
            private readonly State state1, state2;
            private List<StatePair> equivalentPairs;
            private bool isMarked;

            public State State1
            {
                get
                {
                    return state1;
                }
            }
            public State State2
            {
                get
                {
                    return state2;
                }
            }

            public bool IsMarked
            {
                get
                {
                    return isMarked;
                }
            }

            public StatePair(State state1, State state2)
            {
                this.state1 = state1;
                this.state2 = state2;

                this.isMarked = false;

                this.equivalentPairs = new List<StatePair>();
            }

            public void Mark()
            {
                if (isMarked)
                    return;

                isMarked = true;
                foreach (var pair in equivalentPairs)
                    pair.Mark();
            }

            public void AddEquivalentPair(StatePair pair)
            {
                equivalentPairs.Add(pair);
            }

            public static bool operator ==(StatePair pair1, StatePair pair2)
            {
                return (pair1.state1 == pair2.state1 && pair1.state2 == pair2.state2)
                    || (pair1.state2 == pair2.state1 && pair1.state1 == pair2.state2);
            }

            public static bool operator !=(StatePair pair1, StatePair pair2)
            {
                return !(pair1 == pair2);
            }

            public override bool Equals(object obj)
            {
                return obj is StatePair && this == (StatePair)obj;
            }

            public override int GetHashCode()
            {
                // hashcode is order-independent

                int hash1 = state1.GetHashCode(),
                    hash2 = state2.GetHashCode();

                unchecked
                {
                    if (hash1 < hash2)
                        return hash1 + 251 * hash2;
                    else
                        return hash2 + 251 * hash1;
                }
            }

            public bool Equals(StatePair other)
            {
                return this == other;
            }
        }
    }
}

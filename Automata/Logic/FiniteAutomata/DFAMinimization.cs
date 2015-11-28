using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Logic.FiniteAutomata
{
    class DFAMinimization
    {
        private FiniteAutomaton inputDFA, outputMinimizedDFA;

        public DFAMinimization(FiniteAutomaton inputDFA)
        {
            // Using Table-Filling Algorithm
            this.inputDFA = inputDFA;
            List<StatePair> equivalentPairs = GetEquivalentPairs();
            outputMinimizedDFA = RemoveEquivalentPairs(equivalentPairs);
        }

        public FiniteAutomaton GetMinimizedDFA()
        {
            return outputMinimizedDFA;
        }

        private List<StatePair> GetEquivalentPairs()
        {
            List<StatePair> equivalentPairs = new List<StatePair>();
            List<StatePair> initialStatePairs = TableFillingAlgorithmInitiate();
            Alphabet alphabet = inputDFA.GetAlphabet();

            for (int k = 0; k < initialStatePairs.Count; k++)
            {
                StatePair pair = initialStatePairs[k];
                for (int i = 0; i < alphabet.Length; i++)
                {
                    Symbol sym = alphabet[i];

                    StatePair nextPair = GetNextPair(pair, sym);

                    if (initialStatePairs.Contains(nextPair))
                    {
                        if (nextPair.distinguishable == true)
                            MarkDistinguishable(pair);
                        else
                        {
                            if (pair != nextPair)
                                nextPair.AddFollowingPair(pair);
                        }
                    }
                    else
                    {
                        if (nextPair.state1 != nextPair.state2)
                            MarkDistinguishable(pair);
                    }
                }
            }

            foreach (StatePair pair in initialStatePairs)
            {
                if (!pair.distinguishable)
                    equivalentPairs.Add(pair);
            }

            return equivalentPairs;
        }

        private FiniteAutomaton RemoveEquivalentPairs(List<StatePair> equivalentPairs)
        {
            List<State> stateBasket = inputDFA.GetStates().ToList();
            int initialStateIndex = 0;
            List<int> acceptingStateIndexes = new List<int>();
            Alphabet inputAlphabet = inputDFA.GetAlphabet();

            List<Transition> outputTransitions = new List<Transition>();
            List<List<State>> outputStates = new List<List<State>>();
            Dictionary<State, int> stateGroupDictionary = new Dictionary<State, int>();

            while (stateBasket.Count > 0)
            {
                State consideredState = stateBasket[0];

                List<State> outputState = new List<State>();
                outputStates.Add(outputState);

                outputState.Add(consideredState);
                stateGroupDictionary.Add(consideredState, outputStates.Count - 1);
                stateBasket.Remove(consideredState);

                foreach (StatePair pair in equivalentPairs)
                {
                    if (pair.state1 == consideredState)
                    {
                        outputState.Add(pair.state2);
                        stateGroupDictionary.Add(pair.state2, outputStates.Count - 1);
                        stateBasket.Remove(pair.state2);
                    }

                    if (pair.state2 == consideredState)
                    {
                        outputState.Add(pair.state1);
                        stateGroupDictionary.Add(pair.state1, outputStates.Count - 1);
                        stateBasket.Remove(pair.state1);
                    }
                }
            }

            for (int i = 0; i < outputStates.Count; i++)
            {
                List<State> outputState = outputStates[i];
                if (outputState[0].IsAccepting)
                    acceptingStateIndexes.Add(i);
                foreach (State inputState in outputState)
                {
                    if (inputDFA.GetInitialState() == inputState)
                        initialStateIndex = i;
                }

                for (int j = 0; j < inputAlphabet.Length; j++)
                {
                    int nextOutputStateIndex = stateGroupDictionary[GetNextState(outputState[0], inputAlphabet[j])];
                    outputTransitions.Add(new Transition(i, inputAlphabet[j], nextOutputStateIndex));
                }
            }

            FiniteAutomaton minimizedDFA = new FiniteAutomaton(outputStates.Count, inputAlphabet, outputTransitions.ToArray(), initialStateIndex, acceptingStateIndexes.ToArray());
            return minimizedDFA;
        }

        private StatePair GetNextPair(StatePair pair, Symbol sym)
        {
            State[] nextState1 = pair.state1.GetNextStates(sym).ToArray();
            if (nextState1.Length != 1)
                throw new NotSupportedException("Completed DFA Required!");

            State[] nextState2 = pair.state2.GetNextStates(sym).ToArray();
            if (nextState2.Length != 1)
                throw new NotSupportedException("Completed DFA Required!");

            return new StatePair(nextState1[0], nextState2[0]);
        }

        private State GetNextState(State state, Symbol sym)
        {
            State[] nextState = state.GetNextStates(sym).ToArray();
            if (nextState.Length != 1)
                throw new NotSupportedException("Completed DFA Required!");

            return nextState[0];
        }

        private void MarkDistinguishable(StatePair pair)
        {
            pair.distinguishable = true;

            if (pair.followingStatePairs.Count > 0)
                foreach (StatePair followingPair in pair.followingStatePairs)
                {
                    MarkDistinguishable(followingPair);
                }
        }

        private List<StatePair> TableFillingAlgorithmInitiate()
        {
            List<StatePair> initialStatePairs = new List<StatePair>();
            State[] inputStates = inputDFA.GetStates();

            for (int i = 0; i < inputStates.Length - 1; i++)
            {
                for (int j = i + 1; j < inputStates.Length; j++)
                {
                    if ((inputStates[i].IsAccepting && inputStates[j].IsAccepting) || (!inputStates[i].IsAccepting && !inputStates[j].IsAccepting))
                    {
                        initialStatePairs.Add(new StatePair(inputStates[i], inputStates[j]));
                    }
                }
            }

            return initialStatePairs;
        }

        private class StatePair
        {
            public State state1 { get; set; }
            public State state2 { get; set; }
            public List<StatePair> followingStatePairs { get; set; }
            public bool distinguishable;

            public StatePair(State state1, State state2)
            {
                this.state1 = state1;
                this.state2 = state2;
                this.followingStatePairs = new List<StatePair>();
                this.distinguishable = false;
            }

            public void AddFollowingPair(StatePair pair)
            {
                followingStatePairs.Add(pair);
            }
        }
    }
}

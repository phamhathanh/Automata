using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class DFAConverter
    {
        private FiniteAutomaton inputNFA, outputDFA;
        private Dictionary<int, IEnumerable<State>> stateConversionGraph; // Indicates which NFA nodes in a DFA node
        private Queue<int> dfaUnvisitedStateIndexes;

        public DFAConverter(FiniteAutomaton input)
        {
            this.inputNFA = input;

            stateConversionGraph = new Dictionary<int, IEnumerable<State>>();
            dfaUnvisitedStateIndexes = new Queue<int>();
            List<TransitionInfo> dfaTransitionInfos = new List<TransitionInfo>();
            List<int> dfaAcceptingStateIndexes = new List<int>();

            // Initialize graph with δ*(q0, epsilon)
            stateConversionGraph.Add(0, input.PEpsilonClosure(new State[] { input.GetInitialState() }));
            dfaUnvisitedStateIndexes.Enqueue(0);

            Alphabet alphabet = input.GetAlphabet();

            while (dfaUnvisitedStateIndexes.Count != 0)
            {
                int dfaStateIndex = dfaUnvisitedStateIndexes.Dequeue();

                for (int i = 0; i < alphabet.Length; i++)
                {
                    Symbol sym = alphabet[i];
                    IEnumerable<State> correspondingNFAStates = input.Move(stateConversionGraph[dfaStateIndex], sym);
                    // Check if DFA State Exists
                    if (correspondingNFAStates.Count() > 0)
                    {
                        int DFAState = GetDFAStateIndexFromNFAStates(correspondingNFAStates);
                        if (DFAState == -1)
                        {
                            // Create a new one
                            int nextState = stateConversionGraph.Count;

                            // Add to Dictionary
                            stateConversionGraph.Add(nextState, correspondingNFAStates);

                            dfaUnvisitedStateIndexes.Enqueue(nextState);

                            if (IsDFAStateAccepting(nextState))
                                dfaAcceptingStateIndexes.Add(nextState);

                            dfaTransitionInfos.Add(new TransitionInfo(dfaStateIndex, sym, stateConversionGraph.Count - 1));
                        }
                        else
                        {
                            dfaTransitionInfos.Add(new TransitionInfo(dfaStateIndex, sym, DFAState));
                        }
                    }
                }
            }

            outputDFA = new FiniteAutomaton(stateConversionGraph.Count, alphabet, dfaTransitionInfos.ToArray(), 0, dfaAcceptingStateIndexes.ToArray());
        }

        public FiniteAutomaton GetOutputDFA()
        {
            return outputDFA;
        }

        private int GetDFAStateIndexFromNFAStates(IEnumerable<State> newState)
        {
            foreach (var record in stateConversionGraph)
            {
                IEnumerable<State> state = record.Value;
                if (state.Count() == newState.Count() && state.Intersect(newState).Count() == newState.Count())
                    return record.Key;
            }
            return -1;
        }

        private bool IsDFAStateAccepting(int DFAState)
        {
            IEnumerable<State> NFAstates;
            if (stateConversionGraph.TryGetValue(DFAState, out NFAstates))
            {
                foreach (State NFAState in NFAstates)
                {
                    if (NFAState.IsAccepting)
                        return true;
                }
            }
            return false;
        }
    }

    partial class FiniteAutomaton
    {
        /*
         * This part is temporarily used for DFAConverter class
         */
        public State GetInitialState()
        {
            return initialState;
        }

        public Alphabet GetAlphabet()
        {
            return this.alphabet;
        }
        
        public IEnumerable<State> PEpsilonClosure(IEnumerable<State> states)
        {
            return EpsilonClosure(states);
        }
        public IEnumerable<State> Move(IEnumerable<State> states, Symbol symbol)
        {
            // return Extended transition δ*(T, a) for both NFA & DFA
            IEnumerable<State> closure = EpsilonClosure(states);
            List<State> raw_results = new List<State>();
            foreach (State state in closure)
            {
                IEnumerable<State> aStates = state.GetNextStates(symbol);
                foreach (State aState in aStates)
                {
                    if (!raw_results.Contains(aState))
                        raw_results.Add(aState);
                }
            }

            return EpsilonClosure(raw_results);
        }
    }
}

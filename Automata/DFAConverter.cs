using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class DFAConverter
    {
        private FiniteAutomaton input, output;
        private Dictionary<int, IEnumerable<State>> conversionGraph; // Indicates which NFA nodes in a DFA node
        private Queue<int> queue;

        public DFAConverter(FiniteAutomaton input)
        {
            this.input = input;

            conversionGraph = new Dictionary<int, IEnumerable<State>>();
            queue = new Queue<int>();
            List<Transition> transitions = new List<Transition>();
            List<int> accepting_indexes = new List<int>();

            // Initialize graph with δ*(q0, epsilon)
            conversionGraph.Add(0, input.PEpsilonClosure(new State[] { input.GetInitialState() }));
            
            Alphabet alphabet = input.GetAlphabet();

            while (queue.Count != 0)
            {
                int state = queue.Dequeue();

                for (int i = 0; i < alphabet.Length; i++)
                {
                    Symbol sym = alphabet[i];
                    IEnumerable<State> nextSymState = input.Move(conversionGraph[state], sym);
                    // Check if DFA State Exists
                    int DFAState = CheckDFAState(nextSymState);
                    if (DFAState == -1)
                    {
                        // Create a new one
                        int nextState = conversionGraph.Count;
                        
                        // Add to Dictionary
                        conversionGraph.Add(nextState, nextSymState);

                        queue.Enqueue(nextState);

                        if (IsDFAStateAccepting(nextState))
                            accepting_indexes.Add(nextState);

                        transitions.Add(new Transition(state, sym, conversionGraph.Count - 1));
                    }
                    else
                    {
                        transitions.Add(new Transition(state, sym, DFAState));
                    }
                }
            }

            output = new FiniteAutomaton(conversionGraph.Count, alphabet, transitions.ToArray(), 0, accepting_indexes.ToArray());
        }

        public FiniteAutomaton getResult()
        {
            return output;
        }

        private int CheckDFAState(IEnumerable<State> newState)
        {
            foreach (var record in conversionGraph)
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
            if (conversionGraph.TryGetValue(DFAState, out NFAstates))
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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata.Logic
{
    class TransitionTable
    {
        private Dictionary<Tuple<State, char>, List<State>> transitions;
        private bool wrappedUp = false;

        public TransitionTable()
        {
            transitions = new Dictionary<Tuple<State, char>, List<State>>();
        }

        public void Add(State current, char character, State next)
        {
            if (wrappedUp)
                throw new InvalidOperationException("Table is wrapped up.");

            List<State> nextStates;
            var key = new Tuple<State, char>(current, character);

            bool transitionExists = transitions.TryGetValue(key, out nextStates);
            if (!transitionExists)
            {
                nextStates = new List<State>();
                transitions.Add(key, nextStates);
            }

            Debug.Assert(!nextStates.Contains(next));
            nextStates.Add(next);
        }

        public void WrapUp()
        {
            wrappedUp = true;
        }

        public IEnumerable<State> GetAllStates()
        {
            var yielded = new List<State>();
            foreach (var transition in transitions)
            {
                var involved = GetInvolvedStates(transition);

                foreach (var state in involved)
                    if (!yielded.Contains(state))
                    {
                        yield return state;
                        yielded.Add(state);
                    }
            }
        }

        public IEnumerable<char> GetAllCharacters()
        {
            var yielded = new List<char>();
            foreach (var transition in transitions)
            {
                char character = transition.Key.Item2;
                if (!yielded.Contains(character))
                {
                    yield return character;
                    yielded.Add(character);
                }
            }
        }

        public IEnumerable<Tuple<State, char, State>> GetAllTransitions()
        {
            foreach (var transition in transitions)
            {
                State current = transition.Key.Item1;
                char character = transition.Key.Item2;
                var nexts = transition.Value;
                foreach (var next in nexts)
                {
                    yield return new Tuple<State, char, State>(current, character, next);
                }
            }
        }

        private IEnumerable<State> GetInvolvedStates(KeyValuePair<Tuple<State, char>, List<State>> transition)
        {
            State current = transition.Key.Item1;
            var nexts = transition.Value;

            yield return current;
            foreach (var state in nexts)
                yield return state;
        }

        public IEnumerable<State> GetNextStates(IEnumerable<State> states, char character)
        {
            foreach (State state in states)
            {
                var nextStates = GetNextStatesOfSingleState(state, character);
                var closure = EpsilonClosure(nextStates);
                foreach (State nextState in closure)
                    yield return nextState;
            }
        }

        private IEnumerable<State> GetNextStatesOfSingleState(State current, char character)
        {
            List<State> nextStates;
            var key = new Tuple<State, char>(current, character);

            IEnumerable<State> output;
            bool transitionIsDefined = transitions.TryGetValue(key, out nextStates);
            if (transitionIsDefined)
                output = nextStates;
            else
                output = Enumerable.Empty<State>();

            foreach (State state in output)
                yield return state;
        }

        public IEnumerable<State> GetInitialEpsilonClosure(State initial)
        {
            var initialStateArray = new[] { initial };
            return EpsilonClosure(initialStateArray);
        }

        private IEnumerable<State> EpsilonClosure(IEnumerable<State> states)
        {
            var considerations = new Queue<State>(states);
            var closure = new List<State>(considerations.Count);

            while (considerations.Count != 0)
            {
                var state = considerations.Dequeue();
                yield return state;
                closure.Add(state);

                var epsilonStates = GetNextStatesOfSingleState(state, Alphabet.Epsilon);
                foreach (State epsilonState in epsilonStates)
                {
                    bool alreadyConsidered = closure.Contains(epsilonState),
                         isBeingConsidered = considerations.Contains(epsilonState);
                    if (!alreadyConsidered && !isBeingConsidered)
                        considerations.Enqueue(epsilonState);
                }
            }
        }
    }
}

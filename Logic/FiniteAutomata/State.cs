using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Automata.Logic
{
    class State
    {
        private bool isWrappedUp, isAccepting;
        private Dictionary<char, List<State>> transitions = new Dictionary<char, List<State>>();

        public bool IsAccepting
        {
            get
            {
                if (!isWrappedUp)
                    throw new InvalidOperationException("Object is not ready.");

                return isAccepting;
            }
            set
            {
                if (isWrappedUp)
                    throw new InvalidOperationException("Object is wrapped up and cannot be changed.");

                isAccepting = value;
            }
        }

        public void AddTransition(char character, State next)
        {
            if (isWrappedUp)
                throw new InvalidOperationException("Object is wrapped up and cannot be changed.");

            List<State> transition;
            bool transitionIsDefined = transitions.TryGetValue(character, out transition);
            if (!transitionIsDefined)
            {
                transition = new List<State>();
                transitions.Add(character, transition);
            }
            else if (transition.Contains(next))
                throw new InvalidOperationException("Transition is already defined.");

            transition.Add(next);
        }

        public void WrapUp()
        {
            isWrappedUp = true;
        }

        public IEnumerable<State> GetNextStates(char character)
        {
            List<State> transition;
            bool transitionIsDefined = transitions.TryGetValue(character, out transition);
            IEnumerable<State> states;
            if (transitionIsDefined)
                states = transition;
            else
                states = Enumerable.Empty<State>();

            foreach (State state in states)
                yield return state;
        }
    }
}

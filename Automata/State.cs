using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class State
    {
        private bool isWrappedUp, isAccepting;
        private Dictionary<Symbol, List<State>> transitions = new Dictionary<Symbol, List<State>>();

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

        public void AddTransition(Symbol symbol, State next)
        {
            if (isWrappedUp)
                throw new InvalidOperationException("Object is wrapped up and cannot be changed.");

            List<State> transition;
            bool transitionIsDefined = transitions.TryGetValue(symbol, out transition);
            if (!transitionIsDefined)
            {
                transition = new List<State>();
                transitions.Add(symbol, transition);
            }

            transition.Add(next);
        }

        public void WrapUp()
        {
            isWrappedUp = true;
        }

        public IEnumerable<State> GetNextStates(Symbol symbol)
        {
            List<State> transition;
            bool transitionIsDefined = transitions.TryGetValue(symbol, out transition);
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

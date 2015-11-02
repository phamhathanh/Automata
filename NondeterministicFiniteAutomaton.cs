using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class NondeterministicFiniteAutomaton : FiniteAutomaton
    {
        public NondeterministicFiniteAutomaton(State[] states, int initialStateIndex, int[] finalStateIndexes)
            : base(states, initialStateIndex, finalStateIndexes)
        {
            // TODO: construct transition function
        }

        public override bool AcceptString(string input)
        {
            throw new NotImplementedException();
        }

        protected override State Transition(State current, Symbol symbol)
        {
            throw new NotImplementedException();
        }
    }
}

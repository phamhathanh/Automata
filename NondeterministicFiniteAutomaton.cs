using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class NondeterministicFiniteAutomaton : FiniteAutomaton
    {


        public NondeterministicFiniteAutomaton(int statesCount, Alphabet alphabet, Transition[] transitions,
                                                    int initialStateIndex, int[] finalStateIndexes)
            : base(statesCount, alphabet, transitions, initialStateIndex, finalStateIndexes)
        {
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Transition : ITransition
    {
        private readonly State currentState, nextState;
        private readonly Symbol symbol;

        public IState CurrentState
        {
            get
            {
                return currentState;
            }
        }

        public IState NextState
        {
            get
            {
                return nextState;
            }
        }

        public object Symbol
        {
            get
            {
                return symbol;
            }
        }

        public Transition(State currentState, Symbol symbol, State nextState)
        {
            this.currentState = currentState;
            this.symbol = symbol;
            this.nextState = nextState;
        }
    }
}

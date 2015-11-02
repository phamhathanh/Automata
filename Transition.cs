using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Transition
    {
        private readonly State current, next;
        private readonly Symbol symbol;

        public Transition(State current, Symbol symbol, State next)
        {
            this.current = current;
            this.symbol = symbol;
            this.next = next;
        }
    }
}

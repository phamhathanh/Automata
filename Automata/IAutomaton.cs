using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    interface IAutomaton
    {
        IEnumerable<IState> States
        {
            get;
        }

        IEnumerable<ITransition> Transitions
        {
            get;
        }
    }
}

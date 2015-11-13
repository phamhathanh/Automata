using System.Collections.Generic;

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

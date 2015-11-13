using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    interface ITransition
    {
        IState CurrentState
        {
            get;
        }

        IState NextState
        {
            get;
        }

        Symbol Symbol
        {
            get;
        }
    }
}

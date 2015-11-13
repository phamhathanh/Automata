using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class TransitionInfo
    {
        private readonly int currentStateIndex, nextStateIndex;
        private readonly Symbol symbol;

        public int CurrentStateIndex
        {
            get
            {
                return currentStateIndex;
            }
        }

        public int NextStateIndex
        {
            get
            {
                return nextStateIndex;
            }
        }

        public Symbol Symbol
        {
            get
            {
                return symbol;
            }
        }

        public TransitionInfo(int currentStateIndex, Symbol symbol, int nextStateIndex)
        {
            this.currentStateIndex = currentStateIndex;
            this.symbol = symbol;
            this.nextStateIndex = nextStateIndex;
        }
    }
}

namespace Automata
{
    class Transition
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

        public Transition(int currentStateIndex, Symbol symbol, int nextStateIndex)
        {
            this.currentStateIndex = currentStateIndex;
            this.symbol = symbol;
            this.nextStateIndex = nextStateIndex;
        }

        public static bool operator ==(Transition transition1, Transition transition2)
        {
            return transition1 == transition2;
        }

        public static bool operator !=(Transition transition1, Transition transition2)
        {
            return !(transition1 == transition2);
        }

        public override bool Equals(object obj)
        {
            return obj is Transition && this == (Transition)obj;
        }

        public override int GetHashCode()
        {
            return 7 * currentStateIndex.GetHashCode()
                + 17 * symbol.GetHashCode()
                + 37 * nextStateIndex.GetHashCode();
        }
    }
}

namespace Automata.Logic
{
    class State
    {
#if DEBUG
        public override string ToString()
        {
            return GetHashCode().ToString();
        }
#endif
    }
}

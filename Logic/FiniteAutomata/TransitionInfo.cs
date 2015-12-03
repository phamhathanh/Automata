namespace Automata.Logic
{
    public class TransitionInfo
    {
        private readonly int currentStateIndex, nextStateIndex;
        private readonly char character;

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

        public char Symbol
        {
            get
            {
                return character;
            }
        }

        public TransitionInfo(int currentStateIndex, char character, int nextStateIndex)
        {
            this.currentStateIndex = currentStateIndex;
            this.character = character;
            this.nextStateIndex = nextStateIndex;
        }

        public static bool operator ==(TransitionInfo transition1, TransitionInfo transition2)
        {
            return transition1.currentStateIndex == transition2.currentStateIndex
                && transition1.nextStateIndex == transition2.nextStateIndex
                && transition1.character == transition2.character;
        }

        public static bool operator !=(TransitionInfo transition1, TransitionInfo transition2)
        {
            return !(transition1 == transition2);
        }

        public override bool Equals(object obj)
        {
            return obj is TransitionInfo && this == (TransitionInfo)obj;
        }

        public override int GetHashCode()
        {
            return 7 * currentStateIndex.GetHashCode()
                + 17 * character.GetHashCode()
                + 37 * nextStateIndex.GetHashCode();
        }
    }
}

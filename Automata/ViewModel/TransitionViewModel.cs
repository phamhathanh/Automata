using System.ComponentModel;

namespace Automata
{
    class TransitionViewModel : INotifyPropertyChanged
    {
        private string currentStateID, nextStateID;
        private string symbol;

        public string CurrentStateID
        {
            get
            {
                return currentStateID;
            }
            set
            {
                currentStateID = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentStateID"));
                }
            }
        }
        public string Symbol
        {
            get
            {
                return symbol;
            }
            set
            {
                symbol = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Symbol"));
                }
            }
        }
        public string NextStateID
        {
            get
            {
                return nextStateID;
            }
            set
            {
                nextStateID = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("NextStateID"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TransitionViewModel(string currentStateID, string symbol, string nextStateID)
        {
            this.currentStateID = currentStateID;
            this.symbol = symbol;
            this.nextStateID = nextStateID;
        }

        public static bool operator==(TransitionViewModel transition1, TransitionViewModel transition2)
        {
            return transition1.currentStateID == transition2.currentStateID
                && transition1.symbol == transition2.symbol
                && transition1.nextStateID == transition2.nextStateID;
        }

        public static bool operator !=(TransitionViewModel transition1, TransitionViewModel transition2)
        {
            return !(transition1 == transition2);
        }

        public override bool Equals(object obj)
        {
            return obj is TransitionViewModel && this == (TransitionViewModel)obj;
        }

        public override int GetHashCode()
        {
            return 7 * currentStateID.GetHashCode()
                + 17 * symbol.GetHashCode()
                + 37 * nextStateID.GetHashCode();
        }
    }
}

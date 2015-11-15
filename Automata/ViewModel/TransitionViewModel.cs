using System.ComponentModel;

namespace Automata
{
    class TransitionViewModel : INotifyPropertyChanged
    {
        private string currentStateID, symbol, nextStateID;

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
    }
}

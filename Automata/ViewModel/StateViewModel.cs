using System.ComponentModel;

namespace Automata
{
    class StateViewModel : INotifyPropertyChanged
    {
        private string id;
        private bool isAccepting;

        public string ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
                }
            }
        }

        public bool IsAccepting
        {
            get
            {
                return isAccepting;
            }
            set
            {
                isAccepting = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsAccepting"));
                }
            }
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public StateViewModel(string id, bool isAccepting)
        {
            this.id = id;
            this.isAccepting = isAccepting;
        }
    }
}

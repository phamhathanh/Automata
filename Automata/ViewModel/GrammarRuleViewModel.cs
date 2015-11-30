using System.ComponentModel;

namespace Automata
{
    class GrammarRuleViewModel : INotifyPropertyChanged
    {
        private char from;
        private string to;

        public char From
        {
            get
            {
                return from;
            }
            set
            {
                from = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("From"));
                }
            }
        }

        public string To
        {
            get
            {
                return to;
            }
            set
            {
                to = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("To"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public GrammarRuleViewModel(char _from, string _to)
        {
            this.from = _from;
            this.to = _to;
        }

        public static bool operator ==(GrammarRuleViewModel rule1, GrammarRuleViewModel rule2)
        {
            return rule1.from == rule2.from
                && rule1.to == rule2.to;
        }

        public static bool operator !=(GrammarRuleViewModel rule1, GrammarRuleViewModel rule2)
        {
            return !(rule1 == rule2);
        }

        public override bool Equals(object obj)
        {
            return obj is GrammarRuleViewModel && this == (GrammarRuleViewModel)obj;
        }

        public override int GetHashCode()
        {
            return 7 * from.GetHashCode()
                + 17 * to.GetHashCode();
        }
    }
}

using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Automata
{
    class AutomatonViewModel : INotifyPropertyChanged
    {
        private FiniteAutomaton automaton;

        private Graph graph;

        private AlphabetViewModel alphabet;
        private int initialStateIndex;
        private ObservableCollection<StateViewModel> states;
        private ObservableCollection<TransitionViewModel> transitions;

        public AlphabetViewModel Alphabet
        {
            get
            {
                return alphabet;
            }
        }

        public int InitialState
        {
            get
            {
                return initialStateIndex;
            }
            set
            {
                initialStateIndex = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("InitialState"));
                }
            }
        }
        public ObservableCollection<StateViewModel> States
        {
            get
            {
                return states;
            }
            set
            {
                states = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("States"));
                }
            }
        }
        public ObservableCollection<TransitionViewModel> Transitions
        {
            get
            {
                return transitions;
            }
            set
            {
                transitions = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Transitions"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AutomatonViewModel()
        {
            graph = new Graph();
            ResetAll();
        }

        public void AddState(string id, bool isAccepting)
        {
            var state = new StateViewModel(id, isAccepting);
            states.Add(state);
            if (initialStateIndex == -1)
                initialStateIndex = 0;

            Node node = new Node(id);
            graph.AddNode(node);
        }

        public void AddTransition(string currentStateID, string symbol, string nextStateID)
        {
            if (states.Count == 0)
                throw new InvalidOperationException("State collection is empty.");

            transitions.Add(new TransitionViewModel(currentStateID, symbol, nextStateID));
            graph.AddEdge(currentStateID, symbol, nextStateID);
        }

        public void ResetAll()
        {
            this.alphabet = null;

            ResetState();
        }

        public void ResetAlphabet(char[] symbols)
        {
            List<char> alphabet = new List<char>();
            foreach (char symbol in symbols)
                if (char.IsLetterOrDigit(symbol))
                    alphabet.Add(symbol);
            
            if (alphabet.Count == 0)
                throw new ArgumentException("No valid character.");

            this.alphabet = new AlphabetViewModel(alphabet.ToArray());

            ResetState();
        }

        public void ResetState()
        {
            states = new ObservableCollection<StateViewModel>();
            initialStateIndex = -1;

            Node dummy = new Node(" ");
            dummy.IsVisible = false;
            graph.AddNode(dummy);

            ResetTransition();
        }


        public void ResetTransition()
        {
            transitions = new ObservableCollection<TransitionViewModel>();
        }

        public bool AcceptString(string input)
        {
            if (alphabet == null || states.Count == 0 || transitions.Count == 0)
                throw new InvalidOperationException("Automaton is not completed.");

            return true;
        }
    }
}

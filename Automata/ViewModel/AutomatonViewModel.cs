using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Automata.Logic;

namespace Automata
{
    class AutomatonViewModel : INotifyPropertyChanged
    {
        private Graph graph;
        private Node dummy;

        private int initialStateIndex;
        private ObservableCollection<SymbolViewModel> symbols;
        private ObservableCollection<StateViewModel> states;
        private ObservableCollection<TransitionViewModel> transitions;

        public Graph Graph
        {
            get
            {
                return graph;
            }
        }

        public int InitialStateIndex
        {
            get
            {
                return initialStateIndex;
            }
            set
            {
                DrawInitialEdge(initialStateIndex, value);
                initialStateIndex = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("InitialStateIndex"));
                }
            }
        }
        public ObservableCollection<SymbolViewModel> Symbols
        {
            get
            {
                return symbols;
            }
            set
            {
                symbols = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Symbols"));
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
                    PropertyChanged(this, new PropertyChangedEventArgs("States"));
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
                    PropertyChanged(this, new PropertyChangedEventArgs("Transitions"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AutomatonViewModel()
        {
            graph = new Graph();
            graph.Attr.LayerDirection = LayerDirection.LR;
            dummy = new Node(" ");
            dummy.IsVisible = false;
            graph.AddNode(dummy);
            ResetAll();
        }

        public void AddState(string id, bool isAccepting)
        {
            if (IsDuplicated(id))
                throw new ArgumentException("State ID is duplicated.");

            var state = new StateViewModel(id, isAccepting);
            states.Add(state);

            Node node = new Node(id);
            if (isAccepting)
                node.Attr.Shape = Shape.DoubleCircle;
            else
                node.Attr.Shape = Shape.Circle;
            graph.AddNode(node);

            if (InitialStateIndex == -1)
                InitialStateIndex = 0;
        }

        private bool IsDuplicated(string id)
        {
            foreach (var presentState in States)
                if (presentState.ID == id)
                    return true;

            return false;
        }

        public void AddTransition(string currentStateID, char symbol, string nextStateID)
        {
            if (States.Count == 0)
                throw new InvalidOperationException("State collection is empty.");

            if (!IsInAlphabet(symbol))
                throw new ArgumentException("Symbol is not in the alphabet.");

            char symbolChar = symbol;

            TransitionViewModel transition = new TransitionViewModel(currentStateID, symbolChar, nextStateID);
            if (transitions.Contains(transition))
                throw new ArgumentException("Transition is already defined.");

            transitions.Add(transition);
            graph.AddEdge(currentStateID, symbolChar.ToString(), nextStateID);
        }

        private bool IsInAlphabet(char symbol)
        {
            foreach (var presentSymbol in Symbols)
                if (presentSymbol.Symbol == symbol)
                    return true;

            return false;
        }

        public void ResetAll()
        {
            Symbols = new ObservableCollection<SymbolViewModel>();
            ResetStates();
        }

        public void ResetAlphabet(char[] symbols)
        {
            symbols = symbols.Distinct().ToArray();
            Array.Sort(symbols);

            List<SymbolViewModel> validSymbols = new List<SymbolViewModel>();
            validSymbols.Add(new SymbolViewModel('ε'));
            foreach (char symbol in symbols)
                if (char.IsLetterOrDigit(symbol))
                    validSymbols.Add(new SymbolViewModel(symbol));

            Symbols = new ObservableCollection<SymbolViewModel>(validSymbols);
            
            if (this.symbols.Count == 0)
                throw new ArgumentException("No valid character.");

            ResetStates();
        }

        public void ResetStates()
        {
            States = new ObservableCollection<StateViewModel>();
            initialStateIndex = -1;

            ResetGraphNodes();

            ResetTransitions();
        }

        private void ResetGraphNodes()
        {
            if (graph.NodeCount < 2)
                return;

            List<Node> toBeRemoved = new List<Node>(graph.NodeCount);
            foreach (Node node in graph.Nodes)
                if (node.Id != dummy.Id)
                    toBeRemoved.Add(node);
            foreach (Node node in toBeRemoved)
                graph.RemoveNode(node);
        }

        public void ResetTransitions()
        {
            Transitions = new ObservableCollection<TransitionViewModel>();
            ResetGraphEdges();
        }

        public void ResetGraphEdges()
        {
            // assert NodeCount != 0
            if (graph.NodeCount == 1)
                return;

            // assert EdgeCount != 0
            if (graph.EdgeCount == 1)
                return;

            List<Edge> toBeRemoved = new List<Edge>(graph.EdgeCount);
            foreach (Edge edge in graph.Edges)
                if (edge.Source != dummy.Id)
                    toBeRemoved.Add(edge);
            foreach (Edge edge in toBeRemoved)
                graph.RemoveEdge(edge);
        }

        private void DrawInitialEdge(int oldIndex, int newIndex)
        {
            if (newIndex == -1)
                return;

            if (oldIndex != -1)
            {
                graph.RemoveNode(dummy);
                graph.AddNode(dummy);
            }

            Node startingNode = graph.FindNode(States[newIndex].ID);
            Edge initialEdge = new Edge(dummy, startingNode, ConnectionToGraph.Connected);
            graph.AddPrecalculatedEdge(initialEdge);
        }

        public bool AcceptString(string input)
        {
            if (symbols == null || states.Count == 0 || transitions.Count == 0)
                throw new InvalidOperationException("Automaton is not completed.");
            foreach (char c in input)
                if (!IsInAlphabet(c))
                    throw new ArgumentException("Symbol is not in the alphabet.");

            FiniteAutomaton automaton = GenerateAutomaton();
            return automaton.AcceptString(input);
        }

        private FiniteAutomaton GenerateAutomaton()
        {
            List<Symbol> symbols = new List<Symbol>(Symbols.Count);
            foreach (var s in Symbols)
                if (s.Symbol != 'ε')
                    symbols.Add(new Symbol(s.Symbol));
            Alphabet alphabet = new Alphabet(symbols.ToArray());

            List<TransitionInfo> transitions = new List<TransitionInfo>(Transitions.Count);
            foreach (var transition in Transitions)
            {
                int currentStateIndex = IndexFromID(transition.CurrentStateID),
                    nextStateIndex = IndexFromID(transition.NextStateID);
                Symbol symbol = transition.Symbol;
                transitions.Add(new TransitionInfo(currentStateIndex, symbol, nextStateIndex));
            }

            List<int> acceptingStateIndexes = new List<int>();
            foreach (var state in States)
                if (state.IsAccepting)
                    acceptingStateIndexes.Add(IndexFromID(state.ID));

            return new FiniteAutomaton(States.Count, alphabet, transitions.ToArray(),
                                        InitialStateIndex, acceptingStateIndexes.ToArray());
        }

        private int IndexFromID(string stateID)
        {
            return int.Parse(stateID);
        }
    }
}

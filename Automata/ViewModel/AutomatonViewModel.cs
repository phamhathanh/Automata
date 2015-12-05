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
        private const char epsilon = 'ε';

        private Graph graph;
        private Node dummy;

        private int initialStateIndex;
        private ObservableCollection<CharacterViewModel> characters;
        private ObservableCollection<StateViewModel> states;
        private ObservableCollection<TransitionViewModel> transitions;

        private FiniteAutomaton automaton;
        private bool automatonChanged = true;

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
        public ObservableCollection<CharacterViewModel> Characters
        {
            get
            {
                return characters;
            }
            set
            {
                characters = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Characters"));
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

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            automatonChanged = true;
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

        public void AddTransition(string currentStateID, char character, string nextStateID)
        {
            if (States.Count == 0)
                throw new InvalidOperationException("State collection is empty.");

            if (!IsInAlphabet(character))
                throw new ArgumentException("Character is not in the alphabet.");

            TransitionViewModel transition = new TransitionViewModel(currentStateID, character, nextStateID);
            if (transitions.Contains(transition))
                throw new ArgumentException("Transition is already defined.");

            transitions.Add(transition);
            graph.AddEdge(currentStateID, character.ToString(), nextStateID);
        }

        private bool IsInAlphabet(char character)
        {
            foreach (var characterVM in Characters)
                if (characterVM.Character == character)
                    return true;

            return false;
        }

        public void ResetAll()
        {
            Characters = new ObservableCollection<CharacterViewModel>();
            ResetStates();
        }

        public void ResetAlphabet(char[] characters)
        {
            characters = characters.Distinct().ToArray();
            Array.Sort(characters);

            var validCharacters = new List<CharacterViewModel>();
            validCharacters.Add(new CharacterViewModel(epsilon));
            foreach (char character in characters)
                if (char.IsLetterOrDigit(character))
                    validCharacters.Add(new CharacterViewModel(character));

            Characters = new ObservableCollection<CharacterViewModel>(validCharacters);

            if (this.characters.Count == 0)
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
            if (graph.NodeCount < 2)    // 1 dummy node
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
            if (characters == null || states.Count == 0 || transitions.Count == 0)
                throw new InvalidOperationException("Automaton is not completed.");
            foreach (char character in input)
                if (!IsInAlphabet(character))
                    throw new ArgumentException("Character is not in the alphabet.");

            if (automatonChanged)
            {
                GenerateAutomaton();
                automatonChanged = false;
            }
            return automaton.AcceptString(input);
        }

        private void GenerateAutomaton()
        {
            var characters = new List<char>(Characters.Count);
            foreach (var characterVM in Characters)
                if (characterVM.Character != epsilon)
                    characters.Add(characterVM.Character);
            var alphabet = characters.ToArray();

            var transitions = new List<Transition>(Transitions.Count);
            foreach (var transition in Transitions)
            {
                int currentStateIndex = IndexFromID(transition.CurrentStateID),
                    nextStateIndex = IndexFromID(transition.NextStateID);
                char character = transition.Character;
                transitions.Add(new Transition(currentStateIndex, character, nextStateIndex));
            }

            List<int> acceptingStateIndexes = new List<int>();
            foreach (var state in States)
                if (state.IsAccepting)
                    acceptingStateIndexes.Add(IndexFromID(state.ID));

            automaton = new FiniteAutomaton(States.Count, alphabet, transitions.ToArray(),
                                        InitialStateIndex, acceptingStateIndexes.ToArray());
        }

        public void ConvertToDFA()
        {
            if (automatonChanged)
                GenerateAutomaton();

            ResetStates();

            var dfa = automaton.GetEquivalentDFA();

            var acceptings = dfa.AcceptingIndexes;
            int statesCount = dfa.StatesCount;
            for (int i = 0; i < statesCount; i++)
            {
                bool isAccepting = acceptings.Contains(i);
                AddState(i.ToString(), isAccepting);
            }

            var transitions = dfa.Transitions;
            foreach (var transition in transitions)
            {
                string currentStateID = transition.CurrentStateIndex.ToString(),
                        nextStateID = transition.NextStateIndex.ToString();
                char character = transition.Character;
                AddTransition(currentStateID, character, nextStateID);
            }

            automaton = dfa;
            automatonChanged = false;
        }

        private int IndexFromID(string stateID)
        {
            return int.Parse(stateID);
        }
    }
}

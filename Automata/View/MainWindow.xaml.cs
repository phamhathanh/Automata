using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Automata
{
    public partial class MainWindow : Window
    {
        AutomatonViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new AutomatonViewModel();
            this.DataContext = ViewModel;
            //a.ItemsSource = ViewModel.States;

            /*
            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(graphViewPanel);

            
            FiniteAutomaton automaton = TestAutomaton();
            graph = GraphFromAutomaton(automaton);

            graph.Attr.LayerDirection = LayerDirection.LR;

            graphViewer.Graph = graph;

            //InitialStateComboBox.ItemsSource = graph.Nodes;
            */
        }

        /*
        private Graph GraphFromAutomaton(FiniteAutomaton automaton)
        {
            Graph graph = new Graph();

            Dictionary<State, Node> nodesByState = new Dictionary<State, Node>();
            foreach (State state in automaton.States)
            {
                Node node = new Node(nodesByState.Count.ToString());
                if (state.IsAccepting)
                    node.Attr.Shape = Shape.DoubleCircle;
                else
                    node.Attr.Shape = Shape.Circle;

                nodesByState.Add(state, node);
                graph.AddNode(node);
            }
            
            foreach (var transition in automaton.Transitions)
            {
                State currentState = transition.CurrentState,
                          nextState = transition.NextState;
                Node currentNode = nodesByState[currentState],
                        nextNode = nodesByState[nextState];
                object symbol = transition.Symbol;

                Edge edge = new Edge(currentNode, nextNode, ConnectionToGraph.Connected);
                edge.LabelText = symbol.ToString();
                graph.AddPrecalculatedEdge(edge);
            }

            State initialState = automaton.InitialState;
            Node startingNode = nodesByState[initialState],
                dummyNode = new Node(" ");
            dummyNode.IsVisible = false;
            graph.AddNode(dummyNode);
            Edge initialEdge = new Edge(dummyNode, startingNode, ConnectionToGraph.Connected);

            return graph;
        }

        private FiniteAutomaton TestAutomaton()
        {
            Transition[] infos = new Transition[] { new Transition(0, 'a', 1),
                                                            new Transition(0, 'b', 1),
                                                            new Transition(0, 'ε', 3),
                                                            new Transition(0, 'ε', 4),
                                                            new Transition(0, 'ε', 1),
                                                            new Transition(1, 'a', 0),
                                                            new Transition(1, 'ε', 2),
                                                            new Transition(2, 'a', 1),
                                                            new Transition(2, 'a', 2),
                                                            new Transition(3, 'a', 4),
                                                            new Transition(3, 'b', 3),
                                                            new Transition(3, 'ε', 4),
                                                            new Transition(4, 'b', 3) };

            Alphabet alphabet = new Alphabet(new Symbol[] { 'a', 'b' });
            FiniteAutomaton NFA = new FiniteAutomaton(5, alphabet, infos, 0, new int[] { 1 });

            return NFA;
        }*/

        private void addStateButton_Click(object sender, RoutedEventArgs e)
        {
            bool isAccepting = isAcceptingTextBox.IsChecked ?? false;
            string stateID;

            if (stateIDTextBox.Text == "")
                stateID = stateList.Items.Count.ToString();
            else
                stateID = stateIDTextBox.Text;

            ViewModel.AddState(stateID, isAccepting);
        }

        private void addTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            string currentStateID = currentStateComboBox.Text,
                           symbol = symbolComboBox.Text,
                      nextStateID = nextStateComboBox.Text;

            if (!stateList.HasItems)
            {
                MessageBox.Show("Please add some states first.", "Error");
                return;
            }

            if (currentStateID == "" || symbol == "" || nextStateID == "")
            {
                MessageBox.Show("Please select a starting state, a symbol and an ending state.");
                return;
            }
            
            ViewModel.AddTransition(currentStateID, symbol, nextStateID);
        }

        private void resetStateButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetState();
        }

        private void resetTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetTransition();
        }

        private void newAlphabetButton_Click(object sender, RoutedEventArgs e)
        {
            List<char> symbols = new List<char>();
            foreach (char symbol in alphabetTextBox.Text)
                if (char.IsLetterOrDigit(symbol))
                    symbols.Add(symbol);

            if (symbols.Count == 0)
            {
                MessageBox.Show("Please enter at least a valid character.", "Error");
                return;
            }

            try
            {
                ViewModel.ResetAlphabet(symbols.ToArray());
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Please enter at least a valid character.", "Error");
            }
        }
    }
}

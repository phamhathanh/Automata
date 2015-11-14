using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Automata
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(graphPanel);
            graphViewer.LayoutEditingEnabled = false;

            FiniteAutomaton automaton = TestAutomaton();
            Graph graph = GraphFromAutomaton(automaton);

            graph.Attr.LayerDirection = LayerDirection.LR;

            graphViewer.Graph = graph;
        }

        private Graph GraphFromAutomaton(IAutomaton automaton)
        {
            Graph graph = new Graph();

            Dictionary<IState, Node> nodesByState = new Dictionary<IState, Node>();
            foreach (IState state in automaton.States)
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
                IState currentState = transition.CurrentState,
                          nextState = transition.NextState;
                Node currentNode = nodesByState[currentState],
                        nextNode = nodesByState[nextState];
                object symbol = transition.Symbol;

                Edge edge = new Edge(currentNode, nextNode, ConnectionToGraph.Connected);
                edge.LabelText = symbol.ToString();

                if (currentNode != nextNode)
                    currentNode.AddOutEdge(edge);
                else
                    currentNode.AddSelfEdge(edge);
            }

            return graph;
        }

        private FiniteAutomaton TestAutomaton()
        {
            TransitionInfo[] infos = new TransitionInfo[] { new TransitionInfo(0, 'a', 1),
                                                            new TransitionInfo(0, 'b', 1),
                                                            new TransitionInfo(0, 'ε', 3),
                                                            new TransitionInfo(1, 'a', 0),
                                                            new TransitionInfo(1, 'ε', 2),
                                                            new TransitionInfo(2, 'a', 1),
                                                            new TransitionInfo(2, 'a', 2),
                                                            new TransitionInfo(3, 'a', 4),
                                                            new TransitionInfo(3, 'b', 3),
                                                            new TransitionInfo(3, 'ε', 4),
                                                            new TransitionInfo(4, 'b', 3) };

            Alphabet alphabet = new Alphabet(new Symbol[] { 'a', 'b' });
            FiniteAutomaton NFA = new FiniteAutomaton(5, alphabet, infos, 0, new int[] { 1 });

            DFAConverter converter = new DFAConverter(NFA);
            FiniteAutomaton DFA = converter.GetOutputDFA();

            return DFA;
        }
    }
}

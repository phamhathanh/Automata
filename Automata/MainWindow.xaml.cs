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
        DockPanel Panel = new DockPanel();

        public MainWindow()
        {
            InitializeComponent();
            this.Content = Panel;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GraphViewer graphViewer = new GraphViewer();
            graphViewer.BindToPanel(Panel);
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
            TransitionInfo[] infos = new TransitionInfo[] { new TransitionInfo(0, '1', 0),
                                                            new TransitionInfo(0, '1', 2),
                                                            new TransitionInfo(0, 'ε', 1),
                                                            new TransitionInfo(1, '0', 5),
                                                            new TransitionInfo(1, 'ε', 2),
                                                            new TransitionInfo(2, '0', 3),
                                                            new TransitionInfo(3, '1', 1),
                                                            new TransitionInfo(3, 'ε', 4),
                                                            new TransitionInfo(3, 'ε', 5),
                                                            new TransitionInfo(5, '1', 4),
                                                            new TransitionInfo(4, '1', 3) };

            Alphabet alphabet = new Alphabet(new Symbol[] { '0', '1' });
            return new FiniteAutomaton(6, alphabet, infos, 0, new int[] { 4, 5 });
        }
    }
}

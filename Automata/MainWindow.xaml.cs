using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
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

            Graph graph = new Graph();
            var sugiyamaSettings = (SugiyamaLayoutSettings)graph.LayoutAlgorithmSettings;
            sugiyamaSettings.NodeSeparation *= 3;

            var edgeRoutingSettings = new EdgeRoutingSettings();
            graph.LayoutAlgorithmSettings.EdgeRoutingSettings = edgeRoutingSettings;

            graph.AddEdge("0", "1", "0");
            graph.AddEdge("0", "1", "2");
            graph.AddEdge("0", "ε", "1");
            graph.AddEdge("1", "0", "5");
            graph.AddEdge("1", "ε", "2");
            graph.AddEdge("2", "0", "3");
            graph.AddEdge("3", "1", "1");
            graph.AddEdge("3", "ε", "4");
            graph.AddEdge("3", "ε", "5");
            graph.AddEdge("5", "1", "4");
            graph.AddEdge("4", "1", "3");

            graph.Attr.LayerDirection = LayerDirection.TB;

            graph.FindNode("0").Attr.Shape = Shape.Circle;
            graph.FindNode("1").Attr.Shape = Shape.Circle;
            graph.FindNode("2").Attr.Shape = Shape.Circle;
            graph.FindNode("3").Attr.Shape = Shape.Circle;
            graph.FindNode("4").Attr.Shape = Shape.Circle;
            graph.FindNode("5").Attr.Shape = Shape.Circle;

            graph.LayerConstraints.PinNodesToSameLayer(new[] { graph.FindNode("0"), graph.FindNode("1"), graph.FindNode("5") });
            graph.LayerConstraints.PinNodesToSameLayer(new[] { graph.FindNode("2"), graph.FindNode("3"), graph.FindNode("4") });
            graph.LayerConstraints.AddSequenceOfUpDownVerticalConstraint(new[] { graph.FindNode("0"), graph.FindNode("2") });
            graph.LayerConstraints.AddSequenceOfUpDownVerticalConstraint(new[] { graph.FindNode("1"), graph.FindNode("3") });
            graph.LayerConstraints.AddSequenceOfUpDownVerticalConstraint(new[] { graph.FindNode("5"), graph.FindNode("4") });


            graphViewer.Graph = graph;
        }
    }
}

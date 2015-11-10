using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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
            Graph graph = new Graph();

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

            graph.Attr.LayerDirection = LayerDirection.LR;

            graph.FindNode("0").Attr.Shape = Shape.Circle;
            graph.FindNode("1").Attr.Shape = Shape.Circle;
            graph.FindNode("2").Attr.Shape = Shape.Circle;
            graph.FindNode("3").Attr.Shape = Shape.Circle;
            graph.FindNode("4").Attr.Shape = Shape.Circle;
            graph.FindNode("5").Attr.Shape = Shape.Circle;

            graphViewer.Graph = graph;
        }
    }
}

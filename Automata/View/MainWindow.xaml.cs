using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Automata
{
    public partial class MainWindow : Window
    {
        private GraphViewer graphViewer;

        AutomatonViewModel ViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            Program.ShowConsoleUI();
#endif
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = new AutomatonViewModel();
            this.DataContext = ViewModel;

            graphViewer = new GraphViewer();
            graphViewer.BindToPanel(graphViewPanel);
        }

        private void UpdateGraph()
        {
            graphViewer.Graph = ViewModel.Graph;
        }

        private void addStateButton_Click(object sender, RoutedEventArgs e)
        {
            bool isAccepting = isAcceptingTextBox.IsChecked ?? false;
            string stateID = stateID = stateList.Items.Count.ToString();
            
            ViewModel.AddState(stateID, isAccepting);
            UpdateGraph();
        }

        private void addTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            string currentStateID = currentStateComboBox.Text,
                           character = characterComboBox.Text,
                      nextStateID = nextStateComboBox.Text;

            if (!stateList.HasItems)
            {
                MessageBox.Show("Please add some states first.", "Error");
                return;
            }

            if (currentStateID == "" || character == "" || nextStateID == "")
            {
                MessageBox.Show("Please select a starting state, a character and an ending state.");
                return;
            }

            Debug.Assert(character.Length == 1);

            try
            {
                ViewModel.AddTransition(currentStateID, character[0], nextStateID);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Transition is already defined.", "Error");
            }

            UpdateGraph();
        }

        private void resetStateButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetStates();
            UpdateGraph();
        }

        private void resetTransitionButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ResetTransitions();
            UpdateGraph();
        }

        private void newAlphabetButton_Click(object sender, RoutedEventArgs e)
        {
            if (alphabetTextBox.Text == "")
            {
                ViewModel.ResetAll();
                UpdateGraph();
                return;
            }

            string alphabetString = alphabetTextBox.Text;
            var characters = new List<char>(alphabetString.Length);
            foreach (char character in alphabetString)
            {
                if (!char.IsLetterOrDigit(character))
                {
                    MessageBox.Show("Please enter only letters or digits.", "Error");
                    return;
                }
                if (!characters.Contains(character))
                    characters.Add(character);
            }

            if (characters.Count == 0)
            {
                MessageBox.Show("Please enter some characters.", "Error");
                return;
            }

            try
            {
                ViewModel.ResetAlphabet(characters.ToArray());
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Please enter at least a valid character.", "Error");
            }

            UpdateGraph();
        }

        private void stateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGraph();
        }

        private void checkStringButton_Click(object sender, RoutedEventArgs e)
        {
            string input = inputTextBox.Text;
            if (input == "")
            {
                MessageBox.Show("Please enter an input string.", "Error");
                return;
            }

            try
            {
                bool isAccepted = ViewModel.AcceptString(input);

                if (isAccepted)
                    inputTextBox.Background = Brushes.LimeGreen;
                else
                    inputTextBox.Background = Brushes.Red;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Please enter a valid alphabet, some states and some transitions.", "Error");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.ToString());
                // TODO: custom exception
            }
        }

        private void inputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            inputTextBox.Background = SystemColors.WindowBrush;
        }

        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            if (!stateList.HasItems)
            {
                MessageBox.Show("Please add some states first.", "Error");
                return;
            }

            ViewModel.ConvertToDFA();
            UpdateGraph();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (!stateList.HasItems)
            {
                MessageBox.Show("Please add some states first.", "Error");
                return;
            }

            ViewModel.Minimize();
            UpdateGraph();
        }
    }
}

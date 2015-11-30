using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Automata
{
    /// <summary>
    /// Interaction logic for Grammar.xaml
    /// </summary>
    public partial class Grammar : Window
    {
        private GraphViewer graphViewer;

        AutomatonViewModel ViewModel { get; set; }

        public Grammar()
        {
            InitializeComponent();

            var codeCheckerForm = new Automata.View.CodeChecker();
            codeCheckerForm.Show();
        }

        private void Grammar_Loaded(object sender, RoutedEventArgs e)
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

            Debug.Assert(symbol.Length == 1);

            try
            {
                ViewModel.AddTransition(currentStateID, symbol[0], nextStateID);
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

        private void btnNFAToDFA_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NFAtoDFA();
            UpdateGraph();
        }

        private void btnDFAMinimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.DFAMinimize();
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            UpdateGraph();
        }

        private void btnNewCFG_Click(object sender, RoutedEventArgs e)
        {
            List<char> nonTerminals = new List<char>();
            foreach (char symbol in txtCFGNonterminal.Text)
                if (char.IsLetterOrDigit(symbol))
                    nonTerminals.Add(symbol);

            List<char> terminals = new List<char>();
            foreach (char symbol in txtCFGTerminal.Text)
                if (char.IsLetterOrDigit(symbol))
                    terminals.Add(symbol);

            List<char> ruleFroms = new List<char>();
            List<string> ruleTos = new List<string>();
            StringCollection txtRules = GetTextRules();

            foreach (string txtRule in txtRules)
            {
                string[] txtRulesSplitted = txtRule.Split(new string[]{"=>"}, StringSplitOptions.None);
                if (txtRulesSplitted.Length == 2)
                {
                    if (txtRulesSplitted[0].Length == 1 && txtRulesSplitted[1].Length > 0)
                    {
                        ruleFroms.Add(txtRulesSplitted[0][0]);
                        ruleTos.Add(txtRulesSplitted[1]);
                    }
                }
            }

            ViewModel.GrammarReset(nonTerminals.ToArray(), terminals.ToArray(), ruleFroms.ToArray(), ruleTos.ToArray());
        }

        private StringCollection GetTextRules()
        {
            var lines = new StringCollection();
            int lineCount = txtCFGRules.LineCount;
            for (int line = 0; line < lineCount; line++)
                lines.Add(txtCFGRules.GetLineText(line).Replace(Environment.NewLine, String.Empty));

            return lines;
        }

        private void btnCYKCheck_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsCYKAccepted(txtCYKTestString.Text))
                txtCYKTestString.Background = Brushes.LimeGreen;
            else
                txtCYKTestString.Background = Brushes.Red;
        }

        private void btnCFGToGrammar_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CFGtoNFA();
            UpdateGraph();
        }

        private void btnGrammarToCFG_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NFAtoCFG();
        }
    }
}

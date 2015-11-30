using Microsoft.Msagl.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Automata;

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
        private ObservableCollection<SymbolViewModel> cfgNonterminals;
        private ObservableCollection<SymbolViewModel> cfgTerminals;
        private ObservableCollection<GrammarRuleViewModel> cfgGrammarRules;

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

        public ObservableCollection<SymbolViewModel> CFGNonterminals
        {
            get
            {
                return cfgNonterminals;
            }
            set
            {
                cfgNonterminals = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CFGNonterminals"));
            }
        }

        public ObservableCollection<SymbolViewModel> CFGTerminals
        {
            get
            {
                return cfgTerminals;
            }
            set
            {
                cfgTerminals = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CFGTerminals"));
            }
        }

        public ObservableCollection<GrammarRuleViewModel> CFGGrammarRules
        {
            get
            {
                return cfgGrammarRules;
            }
            set
            {
                cfgGrammarRules = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CFGGrammarRules"));
            }
        }

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

        public void NFAtoDFA()
        {
            try
            {
                FiniteAutomaton automaton = GenerateAutomaton();
                DFAConverter converter = new DFAConverter(automaton);
                UpdateViewModelFromAutomaton(converter.GetOutputDFA());
            }
            catch { }
        }

        public void DFAMinimize()
        {
            try
            {
                FiniteAutomaton automaton = GenerateAutomaton();
                Logic.FiniteAutomata.DFAMinimization minimizer = new Logic.FiniteAutomata.DFAMinimization(automaton);
                UpdateViewModelFromAutomaton(minimizer.GetMinimizedDFA());
            }
            catch { }
        }

        private FiniteAutomaton GenerateAutomaton()
        {
            List<Symbol> symbols = new List<Symbol>(Symbols.Count);
            foreach (var s in Symbols)
                if (s.Symbol != 'ε')
                    symbols.Add(new Symbol(s.Symbol));
            Alphabet alphabet = new Alphabet(symbols.ToArray());

            List<Transition> transitions = new List<Transition>(Transitions.Count);
            foreach (var transition in Transitions)
            {
                int currentStateIndex = IndexFromID(transition.CurrentStateID),
                    nextStateIndex = IndexFromID(transition.NextStateID);
                Symbol symbol = transition.Symbol;
                transitions.Add(new Transition(currentStateIndex, symbol, nextStateIndex));
            }

            List<int> acceptingStateIndexes = new List<int>();
            foreach (var state in States)
                if (state.IsAccepting)
                    acceptingStateIndexes.Add(IndexFromID(state.ID));

            return new FiniteAutomaton(States.Count, alphabet, transitions.ToArray(),
                                        InitialStateIndex, acceptingStateIndexes.ToArray());
        }

        private CFG GenerateCFG()
        {
            List<Symbol> _nonterminals = new List<Symbol>();
            foreach (var k in CFGNonterminals)
            {
                _nonterminals.Add(new Symbol(k.Symbol));
            }

            List<Symbol> _terminals = new List<Symbol>();
            foreach (var k in CFGTerminals)
            {
                _terminals.Add(new Symbol(k.Symbol));
            }

            List<Rule> _rules = new List<Rule>();
            int test = 0;
            foreach (var k in CFGGrammarRules)
            {
                Console.WriteLine(test + "");
                test++;
                List<Symbol> _tos = new List<Symbol>();
                foreach (char c in k.To.ToCharArray())
                {
                    _tos.Add(new Symbol(c));
                }
                if (_tos.Count > 0)
                    _rules.Add(new Rule(k.From, _tos.ToArray()));
            }

            return new CFG(_nonterminals, _terminals, _rules);
        }

        private void UpdateViewModelFromAutomaton(FiniteAutomaton automaton)
        {
            ResetAll();
            // Alphabet
            Alphabet newAlphabet = automaton.GetAlphabet();
            List<char> newChars = new List<char>();
            for (int i = 0; i < newAlphabet.Length; i++)
            {
                newChars.Add(newAlphabet[i].ToChar());
            }
            ResetAlphabet(newChars.ToArray());

            // States & Transitions & Accepting Indexes
            State[] newStates = automaton.GetStates();
            for (int i = 0; i < newStates.Length; i++)
            {
                AddState(i.ToString(), newStates[i].IsAccepting);
            }
            for (int i = 0; i < newStates.Length; i++)
            {
                for (int j = 0; j < newAlphabet.Length; j++)
                {
                    State[] nextStates = newStates[i].GetNextStates(newAlphabet[j]).ToArray();
                    if (nextStates.Length > 0)
                    {
                        for (int k = 0; k < nextStates.Length; k++)
                        {
                            var nextState = nextStates[k];
                            int nextStateIndex = Array.IndexOf(newStates, nextState);
                            AddTransition(i.ToString(), newAlphabet[j].ToChar(), nextStateIndex.ToString());
                        }
                    }
                }
            }
        }

        private int IndexFromID(string stateID)
        {
            return int.Parse(stateID);
        }

        public void GrammarReset(char[] nonTerminals, char[] terminals, char[] froms, string[] tos)
        {
            List<SymbolViewModel> nonTerminalModels = new List<SymbolViewModel>();
            foreach (char c in nonTerminals)
                nonTerminalModels.Add(new SymbolViewModel(c));
            CFGNonterminals = new ObservableCollection<SymbolViewModel>(nonTerminalModels);

            List<SymbolViewModel> terminalModels = new List<SymbolViewModel>();
            foreach (char c in terminals)
                terminalModels.Add(new SymbolViewModel(c));
            CFGTerminals = new ObservableCollection<SymbolViewModel>(terminalModels);

            int numOfRules = froms.Length > tos.Length ? tos.Length : froms.Length;

            List<GrammarRuleViewModel> _rules = new List<GrammarRuleViewModel>();

            for (int i = 0; i < numOfRules; i++)
                _rules.Add(new GrammarRuleViewModel(froms[i], tos[i]));
            CFGGrammarRules = new ObservableCollection<GrammarRuleViewModel>(_rules);
        }

        public bool IsCYKAccepted(string testString)
        {
            CYK cykChecker = new CYK(GenerateCFG());

            return cykChecker.isStringAcceptable(testString);
        }

        public void CFGtoNFA()
        {
            List<Symbol> symbols = new List<Symbol>(CFGTerminals.Count);
            List<char> symbolChars = new List<char>(CFGTerminals.Count);
            foreach (var s in CFGTerminals)
            {
                symbols.Add(new Symbol(s.Symbol));
                symbolChars.Add(s.Symbol);
            }
            Alphabet alphabet = new Alphabet(symbols.ToArray());
            List<char> stateChars = new List<char>(CFGNonterminals.Count);
            foreach (var s in CFGNonterminals)
            {
                stateChars.Add(s.Symbol);
            }

            int additionalStateCount = 0;
            List<Transition> transitions = new List<Transition>();
            for (int i = 0; i < CFGGrammarRules.Count; i++)
            {
                char[] ruleToArray = CFGGrammarRules[i].To.ToCharArray();

                int numOfNonterminals = 0;
                int numOfTerminals = 0;

                for (int j = 0; j < ruleToArray.Length; j++)
                {
                    if (symbolChars.Contains(ruleToArray[j]))
                    {
                        numOfTerminals++;
                    }
                    if (stateChars.Contains(ruleToArray[j]))
                    {
                        numOfNonterminals++;
                    }
                }
                Console.WriteLine("Rule" + i + ": " + numOfTerminals + "----" + numOfNonterminals);
                if (numOfTerminals == 0 || numOfNonterminals > 1 || (numOfNonterminals == 1 && !stateChars.Contains(ruleToArray[ruleToArray.Length - 1])))
                    continue;

                int previousIndex = stateChars.IndexOf(CFGGrammarRules[i].From);
                Console.WriteLine("From: " + previousIndex);
                if (previousIndex > -1)
                {
                    for (int j = 0; j < numOfTerminals; j++)
                    {
                        if (j == numOfTerminals - 1)
                        {
                            if (numOfNonterminals == 0)
                            {
                                transitions.Add(new Transition(previousIndex, new Symbol(ruleToArray[j]), CFGNonterminals.Count));
                                Console.WriteLine(previousIndex + "-" + ruleToArray[j] + "-" + CFGNonterminals.Count);
                            }
                            else
                            {
                                transitions.Add(new Transition(previousIndex, new Symbol(ruleToArray[j]), stateChars.IndexOf(ruleToArray[ruleToArray.Length - 1])));
                                Console.WriteLine(previousIndex + "-" + ruleToArray[j] + "-" + stateChars.IndexOf(ruleToArray[ruleToArray.Length - 1]));
                            }
                        }
                        else
                        {
                            transitions.Add(new Transition(previousIndex, new Symbol(ruleToArray[j]), CFGNonterminals.Count + additionalStateCount + 1));
                            Console.WriteLine(previousIndex + "-" + ruleToArray[j] + "-" + (CFGNonterminals.Count + additionalStateCount + 1));
                            previousIndex = CFGNonterminals.Count + additionalStateCount + 1;
                            additionalStateCount++;
                        }
                    }
                }
            }
            FiniteAutomaton automaton = new FiniteAutomaton(CFGNonterminals.Count + 1 + additionalStateCount, alphabet, transitions.ToArray(), 0, new int[] { CFGNonterminals.Count });

            UpdateViewModelFromAutomaton(automaton);
        }

        public void NFAtoCFG()
        {
            string nonterminalChars = "ABCDEFGHIKLMNOPQRSWXYZ";

            char[] nonTerminals = new char[States.Count];
            List<char> terminals = new List<char>();
            char[] transitionFrom = new char[Transitions.Count + 1];
            string[] transitionTo = new string[Transitions.Count + 1];

            for (int i = 0; i < States.Count; i++)
            {
                nonTerminals[i] = nonterminalChars[int.Parse(States[i].ID)];
            }

            for (int i = 0; i < Symbols.Count; i++)
            {
                if (Symbols[i].Symbol != 'ε')
                    terminals.Add(Symbols[i].Symbol);
            }

            for (int i = 0; i < Transitions.Count; i++)
            {
                transitionFrom[i] = nonterminalChars[int.Parse(Transitions[i].CurrentStateID)];
                transitionTo[i] = Transitions[i].Symbol.ToString() + nonterminalChars[int.Parse(Transitions[i].NextStateID)] + "";
            }

            transitionFrom[Transitions.Count] = nonterminalChars[Transitions.Count - 1];
            transitionTo[Transitions.Count] = "ε";

            GrammarReset(nonTerminals, terminals.ToArray(), transitionFrom, transitionTo);
        }
    }
}

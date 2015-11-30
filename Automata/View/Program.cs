using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    static class Program
    {
        private static string[] nonterminals, terminals, alphabet;
        private static ProductionInfo[] rules;
        private static ContextFreeGrammar grammar, normalForm;
        private static string starting;

        public static void ConsoleUI()
        {
            char keyPressed;
            do
            {
                Console.WriteLine();
                Console.WriteLine("Menu:");
                Console.WriteLine("+ Enter nonterminal symbols (n)");
                Console.WriteLine("+ Enter terminal symbols (t)");
                Console.WriteLine("+ Enter production rules (p)");
                Console.WriteLine("+ Display the grammar (d)");
                Console.WriteLine("+ Display Chomsky normal form (c)");
                Console.WriteLine("+ Check if the grammar generates a sentence (g)");
                Console.WriteLine("+ Reset (r)");
                Console.WriteLine("+ Check if a code is uniquely decodable. (u)");
                Console.WriteLine("+ Exit (x)");
                Console.WriteLine();

                Console.Write("Enter your option: ");

                keyPressed = Console.ReadKey().KeyChar;
                Console.WriteLine();
                Console.WriteLine();

                switch (keyPressed)
                {
                    case 'n':
                        EnterNonterminals();
                        break;
                    case 't':
                        EnterTerminals();
                        break;
                    case 'p':
                        EnterRules();
                        break;
                    case 'd':
                        DisplayGrammar();
                        break;
                    case 'c':
                        DisplayNormalForm();
                        break;
                    case 'g':
                        CheckSentence();
                        break;
                    case 'u':
                        CheckUniquelyDecodability();
                        break;
                    case 'x':
                        Console.WriteLine("The program will now exit.");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                        break;
                    default:
                        Console.WriteLine("Invalid key.");
                        break;
                }
            }
            while (keyPressed != 'x');
        }

        private static void EnterNonterminals()
        {
            string message = "Enter number of nonterminal symbols: ";
            int nonterminalsCount = AskForNumberInput(message, 1);

            nonterminals = new string[nonterminalsCount];
            for (int i = 0; i < nonterminalsCount; i++)
            {
                Console.Write("Enter nonterminal symbol number {0}: ", i + 1);
                nonterminals[i] = Console.ReadLine();
                // precheck or something
            }

            message = "Enter the starting symbol: ";
            starting = AskForSymbolInput(message, nonterminals);
        }

        private static void EnterTerminals()
        {
            string message = "Enter number of terminal symbols: ";
            int terminalsCount = AskForNumberInput(message, 1);

            terminals = new string[terminalsCount];
            for (int i = 0; i < terminalsCount; i++)
            {
                Console.Write("Enter terminal symbol number {0}: ", i + 1);
                terminals[i] = Console.ReadLine();
            }
        }

        private static int AskForNumberInput(string message, int minValue)
        {
            while (true)
            {
                Console.Write(message);
                string input = Console.ReadLine();
                int output;
                bool success = int.TryParse(input, out output);

                if (success && output > minValue - 1)
                    return output;

                Console.WriteLine("Please enter a valid number.");
                Console.WriteLine();
            }
        }

        private static string AskForSymbolInput(string message, string[] validSymbols)
        {
            while (true)
            {
                Console.Write(message);
                string symbol = Console.ReadLine();
                bool valid = validSymbols.Contains(symbol);

                if (valid)
                    return symbol;

                Console.WriteLine("Please enter a valid symbol.");
                Console.WriteLine();
            }
        }

        private static void EnterRules()
        {
            if (nonterminals == null || terminals == null)
            {
                Console.WriteLine("Please enter nonterminal and terminal symbols first.");
                return;
            }
            if (alphabet == null)
                alphabet = nonterminals.Concat(terminals).ToArray();

            string message = "Enter number of production rules: ";
            int rulesCount = AskForNumberInput(message, 1);

            rules = new ProductionInfo[rulesCount];
            for (int i = 0; i < rulesCount; i++)
            {
                message = string.Format("Enter original symbol of production rule number {0}: ", i + 1);
                string original = AskForSymbolInput(message, nonterminals);

                message = "Enter number of symbol in the sentence: ";
                int symbolsCount = AskForNumberInput(message, 0);

                string[] sentence = new string[symbolsCount];
                for (int j = 0; j < symbolsCount; j++)
                {
                    message = string.Format("Enter the symbol number {0}: ", j + 1);
                    sentence[j] = AskForSymbolInput(message, alphabet);
                }

                rules[i] = new ProductionInfo(original, sentence);
            }

            grammar = new ContextFreeGrammar(nonterminals, terminals, rules, starting);
            normalForm = grammar.GetChomskyNormalForm();
        }

        private static void DisplayGrammar()
        {
            if (grammar == null)
            {
                Console.WriteLine("Please enter a grammar first.");
                return;
            }

            foreach (string rule in grammar.Rules)
                Console.WriteLine(rule);
        }

        private static void DisplayNormalForm()
        {
            if (grammar == null)
            {
                Console.WriteLine("Please enter a grammar first.");
                return;
            }

            foreach (string rule in normalForm.Rules)
                Console.WriteLine(rule);
        }

        private static void CheckSentence()
        {
            if (grammar == null)
            {
                Console.WriteLine("Please enter a grammar first.");
                return;
            }

            string[] symbols;
            while (true)
            {
                Console.Write("Enter the sentence to be checked (separate by space): ");
                string input = Console.ReadLine();
                symbols = input.Split(' ');
                foreach (string symbol in symbols)
                    if (!terminals.Contains(symbol))
                    {
                        Console.WriteLine("Please enter a valid sentence.");
                        continue;
                    }
                break;
            }

            bool isMember = normalForm.HasSentence(symbols);
            Console.WriteLine("Result: This sentence is {0}generated by the grammar.", (isMember) ? "" : "not ");
        }

        private static void Reset()
        {
            nonterminals = null;
            terminals = null;
            rules = null;
        }

        private static void CheckUniquelyDecodability()
        {
            RegularLanguage language;
            while (true)
            {
                Console.Write("Enter the regular expression of the code: ");
                string input = Console.ReadLine();
                try
                {
                    language = new RegularLanguage(input);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
                break;
            }

            Patterson code = new Patterson(language);
            Console.WriteLine("Result: The code is {0}uniquely decodable.", (code.IsUniquelyDecodable()) ? "" : "not ");
        }
    }
}

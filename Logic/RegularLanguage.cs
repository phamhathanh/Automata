using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Automata.Logic
{
    public class RegularLanguage
    {
        public static readonly RegularLanguage Empty = new RegularLanguage("");
        private const char emptyCharacter = ' ';

        private readonly Alphabet alphabet;

        public RegularLanguage(string expression)
        {
            Regex validExpression = new Regex(@"^[\w\(\)\|\*]*$");
            bool expressionIsValid = validExpression.IsMatch(expression);
            if (!expressionIsValid)
                throw new ArgumentException("Expression is invalid.");

            var symbols = new List<Symbol>();
            foreach (char c in expression)
            {
                bool isSymbol = !IsOperator(c) && c != '(' && c != ')';
                if (!isSymbol)
                {
                    if (!char.IsLetterOrDigit(c))
                        throw new ArgumentException("Expression must contains only letters, digits and operators.");

                    symbols.Add(c);
                }
            }
            alphabet = new Alphabet(symbols.ToArray());

            // TODO: check expression meaning validity
            //       build a new Automaton class
        }

        public bool Contains(string input)
        {
            throw new Exception();
        }

        private string ProcessExpression(string expression)
        {
            expression = InsertConcatOperator(expression);
            expression = InsertEmptyCharacter(expression);
            return expression;
        }

        private string InsertConcatOperator(string expression)
        {
            StringBuilder output = new StringBuilder();

            bool canConcat = false;
            foreach (char character in expression)
            {
                bool canBeConcatened = character == '(' || char.IsLetterOrDigit(character);

                if (canConcat && canBeConcatened)
                    output.Append('.');

                output.Append(character);

                canConcat = character != '|' && character != '(';
            }

            return output.ToString();
        }

        private string InsertEmptyCharacter(string expression)
        {
            if (expression == "")
                return " ";

            StringBuilder output = new StringBuilder();

            bool mayImplyFollowingEmptyChar = true;
            foreach (char character in expression)
            {
                bool mayImplyPrecedingEmptyChar = character == '|' || character == ')';

                if (mayImplyFollowingEmptyChar && mayImplyPrecedingEmptyChar)
                    output.Append(emptyCharacter);

                output.Append(character);

                mayImplyFollowingEmptyChar = character == '|' || character == '(';
            }

            return output.ToString();
        }

        private string ToPostfixNotation(string infixNotation)
        {
            // Shunting-yard algorithm

            var operatorStack = new Stack<char>();
            var output = new StringBuilder();

            foreach (char character in infixNotation)
            {
                if (IsOperator(character))
                {
                    if (character == '*')
                        output.Append(character);
                    else
                    {
                        while (operatorStack.Count > 0 && OperatorHasMorePriority(operatorStack.Peek(), character))
                            output.Append(operatorStack.Pop());
                        operatorStack.Push(character);
                    }
                }
                else if (character == '(')
                    operatorStack.Push(character);
                else if (character == ')')
                {
                    char topOperator = operatorStack.Pop();
                    while (topOperator != '(')
                    {
                        output.Append(topOperator);
                        topOperator = operatorStack.Pop();
                    }
                }
                else
                    output.Append(character);
            }

            while (operatorStack.Count > 0)
            {
                char character = operatorStack.Pop();
                output.Append(character);
            }

            return output.ToString();
        }

        private bool IsOperator(char character)
        {
            return character == '*' || character == '|' || character == '.';
        }

        private bool OperatorHasMorePriority(char operator1, char operator2)
        {
            Debug.Assert(operator2 != '*');
            Debug.Assert(operator1 != '*');
            Debug.Assert(IsOperator(operator2));
            if (operator1 == '(')
                return false;
            Debug.Assert(IsOperator(operator1));

            return operator1 == '.' && operator2 == '|';
        }

        private FiniteAutomaton EvaluatePostfixNotation(string postfixNotation)
        {
            Debug.Assert(postfixNotation.Length > 0);

            var stack = new Stack<FiniteAutomaton>();
            foreach (char character in postfixNotation)
            {
                FiniteAutomaton result;
                if (IsOperator(character))
                {
                    if (character == '*')
                    {
                        if (stack.Count < 1)
                            throw new ArgumentException("Not enough values in expression.");

                        var operand = stack.Pop();
                        result = Star(operand);
                    }
                    else if (character == '|')
                    {
                        if (stack.Count < 2)
                            throw new ArgumentException("Not enough values in expression.");

                        var operand1 = stack.Pop();
                        var operand2 = stack.Pop();
                        result = Union(operand1, operand2);
                    }
                    else
                    {
                        if (stack.Count < 2)
                            throw new ArgumentException("Not enough values in expression.");

                        var operand1 = stack.Pop();
                        var operand2 = stack.Pop();
                        result = Concat(operand1, operand2);
                    }
                }
                else
                    result = FromSymbol(character);

                stack.Push(result);
            }

            int stackCount = stack.Count();
            Debug.Assert(stackCount > 0);
            if (stackCount > 1)
                throw new ArgumentException("Too many values in expression.");

            return stack.Peek();
        }

        private FiniteAutomaton FromSymbol(char c)
        {
            int statesCount = 2,
                initialStateIndex = 0;
            var finalStateIndexes = new[] { 1 };

            TransitionInfo[] transitions;
            if (c == ' ')
                transitions = new TransitionInfo[] { new TransitionInfo(0, Alphabet.Epsilon, 1) };
            else
            {
                Debug.Assert(alphabet.Contains(c));
                transitions = new TransitionInfo[] { new TransitionInfo(0, c, 1) };
            }

            return new FiniteAutomaton(statesCount, alphabet, transitions, initialStateIndex, finalStateIndexes);
        }

        private FiniteAutomaton Union(FiniteAutomaton operand1, FiniteAutomaton operand2)
        {
            throw new NotImplementedException();
        }

        private FiniteAutomaton Concat(FiniteAutomaton operand1, FiniteAutomaton operand2)
        {
            throw new NotImplementedException();
        }

        private FiniteAutomaton Star(FiniteAutomaton operand)
        {
            throw new NotImplementedException();
        }
    }
}

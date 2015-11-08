using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Program
    {
        static void Main(string[] args)
        {
            Alphabet alphabet = new Alphabet(new Symbol[] { '0', '1' });

            /*
            Transition[] transitions = new Transition[] { new Transition(0, '0', 0),
                                                          new Transition(0, '0', 1),
                                                          new Transition(0, '1', 0),
                                                          new Transition(1, '1', 2),
                                                          new Transition(2, '0', 3),
                                                          new Transition(3, '0', 3),
                                                          new Transition(3, '1', 3) };

            FiniteAutomaton automaton = new FiniteAutomaton(4, alphabet, transitions, 0, new int[] { 3 });
            */

            Transition[] transitions = new Transition[] { new Transition(0, '1', 0),
                                                          new Transition(0, '1', 2),
                                                          new Transition(0, 'ε', 1),
                                                          new Transition(1, '0', 5),
                                                          new Transition(1, 'ε', 2),
                                                          new Transition(2, '0', 3),
                                                          new Transition(3, '1', 1),
                                                          new Transition(3, 'ε', 4),
                                                          new Transition(3, 'ε', 5),
                                                          new Transition(4, '0', 3),
                                                          new Transition(5, '1', 4) };
            FiniteAutomaton automaton = new FiniteAutomaton(6, alphabet, transitions, 0, new int[] { 3, 4 });

            Console.WriteLine();

            while (true)
            {
                string testString = Console.ReadLine();
                Console.WriteLine(automaton.AcceptString(testString));
            }
        }
    }
}

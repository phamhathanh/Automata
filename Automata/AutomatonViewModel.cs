using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class AutomatonViewModel
    {
        private Graph graph;
        private FiniteAutomaton automaton;

        public AutomatonViewModel()
        {
            graph = new Graph();
        }
    }
}

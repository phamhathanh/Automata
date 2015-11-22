using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Production
    {
        private readonly string original;
        private readonly string[] directDerivation;

        public string Original
        {
            get
            {
                return original;
            }
        }

        public IEnumerable<string> DirectDerivation
        {
            get
            {
                foreach (string symbol in directDerivation)
                    yield return symbol;
            }
        }

        public Production(string original, string[] directDerivation)
        {
            this.original = original;
            this.directDerivation = (string[])directDerivation.Clone();
        }

        public static bool operator ==(Production production1, Production production2)
        {
            return production1.original == production2.original
                && production1.directDerivation == production2.directDerivation;
        }

        public static bool operator !=(Production production1, Production production2)
        {
            return !(production1 == production2);
        }

        public override bool Equals(object obj)
        {
            return obj is Production && this == (Production)obj;
        }

        public override int GetHashCode()
        {
            return 7 * original.GetHashCode()
                + 37 * directDerivation.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    class Production : IEquatable<Production>
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
            if (production1.original != production2.original)
                return false;

            if (production1.directDerivation.Length != production2.directDerivation.Length)
                return false;

            for (int i = 0; i < production1.directDerivation.Length; i++)
                if (production1.directDerivation[i] != production2.directDerivation[i])
                    return false;

            return true;
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

        public bool Equals(Production other)
        {
            return this == other;
        }
    }
}

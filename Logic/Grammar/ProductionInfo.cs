using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automata
{
    public class ProductionInfo
    {
        private readonly string original;
        private readonly string[] directDerivative;

        public string Original
        {
            get
            {
                return original;
            }
        }

        public IEnumerable<string> DirectDerivative
        {
            get
            {
                foreach (string symbol in directDerivative)
                    yield return symbol;
            }
        }

        public ProductionInfo(string original, string[] directDerivation)
        {
            this.original = original;
            this.directDerivative = (string[])directDerivation.Clone();
        }

        public static bool operator ==(ProductionInfo production1, ProductionInfo production2)
        {
            if (production1.original != production2.original)
                return false;

            if (production1.directDerivative.Length != production2.directDerivative.Length)
                return false;

            for (int i = 0; i < production1.directDerivative.Length; i++)
                if (production1.directDerivative[i] != production2.directDerivative[i])
                    return false;

            return true;
        }

        public static bool operator !=(ProductionInfo production1, ProductionInfo production2)
        {
            return !(production1 == production2);
        }

        public override bool Equals(object obj)
        {
            return obj is ProductionInfo && this == (ProductionInfo)obj;
        }

        public override int GetHashCode()
        {
            return 7 * original.GetHashCode()
                + 37 * directDerivative.GetHashCode();
        }
    }
}

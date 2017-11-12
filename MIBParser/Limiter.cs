using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBParser
{
    public class Limiter
    {
        private int min;
        private int max;
        public Limiter(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public bool Check(int value)
        {
            if (min <= value && value <= max) return true;
            return false;
        }

        public bool Check(string value)
        {
            if (value.Length <= max) return true;
            return false;
        }
    }
}

namespace MIBParser
{
    public class Limiter
    {
        private readonly int max;
        private readonly int min;

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
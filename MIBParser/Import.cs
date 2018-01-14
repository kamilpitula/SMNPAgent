using System.Collections.Generic;

namespace MIBParser
{
    public class Import
    {
        public Import(string name, IEnumerable<string> types)
        {
            Name = name;
            Types = types;
        }

        public string Name { get; }
        public IEnumerable<string> Types { get; }
    }
}
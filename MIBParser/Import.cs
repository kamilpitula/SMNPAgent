using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBParser
{
    public class Import
    {
        public string Name { get; private set; }
        public IEnumerable<string> Types { get; private set; }

        public Import(string name, IEnumerable<string> types)
        {
            Name = name;
            Types = types;
        }
    }
}

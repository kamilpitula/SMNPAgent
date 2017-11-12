using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MIBParser;

namespace SMNPAgentStage1
{
    class Program
    {
        static void Main(string[] args)
        {

            var fileReader = new FileReader();

            var parser = new Parser(fileReader);
            var root = parser.GenerateTree();

            Console.WriteLine(root.GetTreeString("",true));
            Console.WriteLine(" ");
            //Console.WriteLine(root.GetString());
            Console.WriteLine(((ObjectType)root.GetMibNodeStack().FirstOrDefault(node => node.NodeName == "sysDescr"))?.ToString());

            Console.ReadKey();
        }
    }
}

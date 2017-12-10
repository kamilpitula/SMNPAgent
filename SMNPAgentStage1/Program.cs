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
            var importsLoader = new ImportsLoader(fileReader);

            var parser = new Parser(fileReader, importsLoader);
            var root = parser.GenerateTree();

            Console.WriteLine(root.GetTreeString("", true));
            Console.WriteLine(" ");

            while (true)
            {
                Console.WriteLine("Write node name or OID: ");
                var nodeName = Console.ReadLine();

                try
                {
                    Console.WriteLine(root.GetMibNodeStack().FirstOrDefault(node => node.NodeName == nodeName)?.ToString());

                    Console.WriteLine(root.GetMibNodeStack().FirstOrDefault(node => node.GetOID() == nodeName)?.ToString());         
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Node doesn't exist");
                }
            }
        }
    }
}

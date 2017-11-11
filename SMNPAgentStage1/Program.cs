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

            //var importsLoader = new ImportsLoader(fileReader);
            //importsLoader.GetAllFiles(ParserConst.MIBPath);L


            var parser = new Parser(fileReader);
            var root = parser.GenerateTree();

            Console.ReadKey();
        }
    }
}

using System;
using MIBParser;

namespace SMNPAgentStage1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var fileReader = new FileReader();
            var importsLoader = new ImportsLoader(fileReader);

            var parser = new Parser(fileReader, importsLoader);
            var root = parser.GenerateTree();

            Console.WriteLine(root.GetTreeString("", true));
            Console.WriteLine(" ");

            var messageProcessor = new SNMPProcessor(new BerDecoder(), new BerCoder(), root);

            var server = new SNMPServer(messageProcessor);

            Console.WriteLine();
            Console.WriteLine("START");

            var snmpTask = server.RunRecieverLoop();

            snmpTask.Start();

            Console.WriteLine("Press any key to stop");
            Console.ReadKey();
            snmpTask.Wait();
            //while (true)
            //{
            //    Console.WriteLine("Write node name or OID: ");
            //    var nodeName = Console.ReadLine();

            //    try
            //    {
            //        Console.WriteLine(root.GetMibNodeStack().FirstOrDefault(node => node.NodeName == nodeName)?.ToString());

            //        Console.WriteLine(root.GetMibNodeStack().FirstOrDefault(node => node.GetOID() == nodeName)?.ToString());
            //    }
            //    catch (NullReferenceException)
            //    {
            //        Console.WriteLine("Node doesn't exist");
            //    }
            //}
        }
    }
}
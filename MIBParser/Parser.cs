using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MIBParser
{
    public class Parser
    {
        Regex ObjectIdentifierRegex = new Regex(@"(?<name>.*)OBJECT IDENTIFIER ::= {(?<parent>.*)}");
        Regex ObjectTypeRegex = new Regex(@"(?=[a-z|A-Z](.*)OBJECT-TYPE\n)(?s).*?(?>::= { [a-z|A-Z|0-9]* [0-9]* })");

        Regex NameOfNode = new Regex(@"(.*)(?= OBJECT-TYPE)");
        Regex TypeOfNode = new Regex(@"(?<=SYNTAX  )(.*)");
        Regex TypeOfAccess = new Regex(@"(?<=ACCESS  )(.*)");
        Regex ParentAndId = new Regex(@"(?<=::= { )(.*) ");
        Regex SplitSpace = new Regex(@"[a-z|A-Z|0-9]*(?= )");

        Regex ComplexTypeOfNode = new Regex(@"(?=(.*)SYNTAX(.*)\n)(?s).*?(?>})");
        Regex SplitLines = new Regex(@"(?<=)([a-z].*)(?=\))");
        Regex ExtraSplit = new Regex(@"[a-z]*\([0-9]");

        Regex SplitDotNumbers = new Regex(@"\(([0-9]*)..([0-9]*)\)");
        Regex GetMinMaxNumbers = new Regex(@"[0-9]*\)");

        private readonly IFileReader fileReader;

        public Parser(IFileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        public MIBNode GenerateTree()
        {
            //Hardcoded roots of the tree
            var masterNode = new MIBNode(1, "ISO");
            masterNode.AddChild(new MIBNode(3, "org"));

            var org = masterNode.GetMibNodeStack().Where(node=>node.NodeName=="org").FirstOrDefault();
            org.AddChild(new MIBNode(6,"dod"));

            var dod = masterNode.GetMibNodeStack().Where(node => node.NodeName == "dod").FirstOrDefault();
            dod.AddChild(new MIBNode(1, "internet"));

            var internet = masterNode.GetMibNodeStack().Where(node => node.NodeName == "internet").FirstOrDefault();
            internet.AddChild(new MIBNode(2, "mgmt"));
            
            

            string mibText = fileReader.GetFileEntireText(ParserConst.MIBPath);

            var objectIdentifierMatch = ObjectIdentifierRegex.Matches(mibText);

            foreach (Match match in objectIdentifierMatch)
            {
                var groups = match.Groups;

                string name = groups["name"].ToString().Trim(' ');
                string parent = groups["parent"].ToString().Split(' ')[1].Trim(' ');
                string valueString = groups["parent"].ToString().Split(' ')[2].Trim(' ');
                var value = int.Parse(valueString);

                //Console.WriteLine("Nazwa: {0}, Parent: {1}, Numer: {2}",groups["name"].ToString(),parent,value);
                var parentNode = masterNode.GetMibNodeStack().Where(node => node.NodeName == parent).FirstOrDefault();
                parentNode.AddChild(new MIBNode(value,name));
            }
            return masterNode;
        }

    }
}

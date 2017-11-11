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
        Regex ObjectTypeRegex = new Regex(@"(?=[a-z|A-Z](.*)OBJECT-TYPE)(?s).*?(?>::= { [a-z|A-Z|0-9]* [0-9]* })");

        Regex NameOfNode = new Regex(@"((?<name>.*)OBJECT-TYPE)");
        Regex TypeOfNode = new Regex(@"(?<=SYNTAX  )(?<syntax>.*)");
        Regex TypeOfAccess = new Regex(@"(?<=ACCESS  )(?<access>.*)");
        Regex Status = new Regex(@"(?<=STATUS  )(?<status>.*)");
        Regex Description = new Regex(@"(?<=DESCRIPTION  )(?<description>.*)");
        Regex ParentAndId = new Regex(@"(?<=::= { )(?<parent>.*) (?<parentId>\d+)");
        //Regex SplitSpace = new Regex(@"[a-z|A-Z|0-9]*(?= )");

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
            var masterNode = new MIBNode(1, "ISO", null);
            masterNode.AddChild(new MIBNode(3, "org", masterNode));

            var org = masterNode.GetMibNodeStack().Where(node => node.NodeName == "org").FirstOrDefault();
            org.AddChild(new MIBNode(6, "dod", org));

            var dod = masterNode.GetMibNodeStack().Where(node => node.NodeName == "dod").FirstOrDefault();
            dod.AddChild(new MIBNode(1, "internet", dod));

            var internet = masterNode.GetMibNodeStack().Where(node => node.NodeName == "internet").FirstOrDefault();
            internet.AddChild(new MIBNode(2, "mgmt", internet));



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
                parentNode.AddChild(new MIBNode(value, name, parentNode));
            }

            var objectTypeMatch = ObjectTypeRegex.Matches(mibText);
            foreach (Match match in objectTypeMatch)
            {
                var objectTypeText = match.Value;
                var name = NameOfNode.Match(objectTypeText).Groups["name"];
                var typeOfNode = TypeOfNode.Match(objectTypeText).Groups["syntax"];
                var access = TypeOfAccess.Match(objectTypeText).Groups["access"];
                var status = Status.Match(objectTypeText).Groups["status"];
                var description = Description.Match(objectTypeText).Groups["description"];
                var parent = ParentAndId.Match(objectTypeText).Groups["parent"];
                var id = ParentAndId.Match(objectTypeText).Groups["parentId"];

                Console.WriteLine($"Name: {name}, type: {typeOfNode}, access: {access}, status: {status}, description: {description}, parent: {parent}, Id: {id}");
            }


            return masterNode;
        }

    }
}
